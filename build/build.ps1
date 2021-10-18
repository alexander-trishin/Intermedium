[CmdletBinding(PositionalBinding=$false)]
param(
    [Version] $IntermediumVersion = "2.0.0.0",
    [Version] $IntermediumMSDIVersion = "2.0.0.0",

    [Array] $ProjectsToPack = @(
        'Intermedium',
        'Intermedium.Extensions.Microsoft.DependencyInjection'
    ),

    [Array] $TestsToRun = @(
        'Intermedium.Tests',
        'Intermedium.Extensions.Microsoft.DependencyInjection.Tests'
    ),

    [switch] $TreatWarningsAsErrors = $true,
    [switch] $BuildNuGet = $true,

    [ValidateSet("q", "quiet", "m", "minimal", "n", "normal", "d", "detailed", "diag", "diagnostic", IgnoreCase = $false)]
    [string] $MSBuildVerbosity = "m",

    [Uri] $DotNetInstallUrl = "https://dot.net/v1/dotnet-install.ps1",
    [Uri] $NuGetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe",

    [Version] $VSTestVersion = "16.4.0",
    [Version] $VSWhereVersion = "2.3.2",
    [Version] $DotNetVersion = "5.0.402",
    [string] $DotNetChannel = "Current",

    [string] $BaseDirectory = (Resolve-Path ..)
)

$MajorI = $IntermediumVersion.Major
$MinorI = $IntermediumVersion.Minor
$BuildI = $IntermediumVersion.Build

$MajorIEMDI = $IntermediumMSDIVersion.Major
$MinorIEMDI = $IntermediumMSDIVersion.Minor
$BuildIEMDI = $IntermediumMSDIVersion.Build

$BuildDirectory = [IO.Path]::Combine($BaseDirectory, "build")
$ToolsDirectory = [IO.Path]::Combine($BuildDirectory, "tools")
$VSTestDirectory = [IO.Path]::Combine($ToolsDirectory, "Microsoft.TestPlatform.$VSTestVersion")
$VSWhereDirectory = [IO.Path]::Combine($ToolsDirectory, "vswhere.$VSWhereVersion")
$OutputDirectory = [IO.Path]::Combine($BuildDirectory, "bin")
$OutputNuGetDirectory = [IO.Path]::Combine($OutputDirectory, "nuget")
$OutputTestResultsDirectory = [IO.Path]::Combine($OutputDirectory, "test-results")

$SolutionPath = [IO.Path]::Combine($BaseDirectory, "Intermedium.sln")
$NuGetPath = [IO.Path]::Combine($ToolsDirectory, "nuget.exe")
$NuGetConfigPath = [IO.Path]::Combine($BuildDirectory, "nuget.config")
$DotNetInstallPath = [IO.Path]::Combine($ToolsDirectory, "dotnet-install.ps1")

function Say-Invocation($Invocation) {
    $Command = $Invocation.MyCommand;
    $Arguments = (($Invocation.BoundParameters.Keys | ForEach { "-$_ `"$($Invocation.BoundParameters[$_])`"" }) -join " ")
    Write-Host "`nIntermedium: $Command $Arguments"
}

function Locate([string] $SearchDirectory, [string] $SearchFileName) {
    Say-Invocation $MyInvocation

    $Result = Get-ChildItem -Path $SearchDirectory -Include $SearchFileName -Recurse | Select -First 1
    
    if ($Result) {
        return $Result.FullName
    }

    return $null
}

function Invoke-With-Retry([ScriptBlock] $ScriptBlock, [int] $MaxAttempts = 3, [int] $SecondsBetweenAttempts = 1) {
    $Attempts = 0

    while ($true) {
        try {
            return $ScriptBlock.Invoke()
        }
        catch {
            $Attempts++
            if ($Attempts -lt $MaxAttempts) {
                Start-Sleep $SecondsBetweenAttempts
            }
            else {
                throw
            }
        }
    }
}

function Download-File([Uri] $Uri, [string] $SavePath) {
    Say-Invocation $MyInvocation

    if (-Not (Test-Path $SavePath)) {
        Invoke-With-Retry({
            (New-Object System.Net.WebClient).DownloadFile($Uri, $SavePath)
        })
    }
}

function Install-DotNet() {
    Say-Invocation $MyInvocation

    Download-File -Uri $DotNetInstallUrl -SavePath $DotNetInstallPath
    & $DotNetInstallPath -Channel $DotNetChannel -Version $DotNetVersion -NoPath
}

function Install-NuGet() {
    Say-Invocation $MyInvocation

    Download-File -Uri $NuGetUrl -SavePath $NuGetPath
}

function Install-NuGet-Package([string] $PackageName, [Version] $PackageVersion, [string] $SaveDirectory) {
    Say-Invocation $MyInvocation

    Invoke-With-Retry({
        & $NuGetPath install $PackageName -Version $PackageVersion -ConfigFile $NuGetConfigPath -Source "NuGet.org (v3)" -OutputDirectory $SaveDirectory
    })
}

function Install-VSTest() {
    Say-Invocation $MyInvocation

    Install-NuGet-Package "Microsoft.TestPlatform" -PackageVersion $VSTestVersion -SaveDirectory $ToolsDirectory

    $Path = Locate -SearchDirectory $VSTestDirectory -SearchFileName "vstest.console.exe"

    Set-Variable -Name "VSTestPath" -Value $Path -Scope Script
}

function Install-VSWhere() {
    Say-Invocation $MyInvocation

    Install-NuGet-Package "vswhere" -PackageVersion $VSWhereVersion -SaveDirectory $ToolsDirectory

    $Path = Locate -SearchDirectory $VSWhereDirectory -SearchFileName "vswhere.exe"

    Set-Variable -Name "VSWherePath" -Value $Path -Scope Script
}

function Locate-MSBuild() {
    Say-Invocation $MyInvocation

    if ($VSWherePath) {
        $VSInstallDirectory = & $VSWherePath -latest -products * -requires Microsoft.Component.MSBuild -property installationPath

        if ($VSInstallDirectory) {
            foreach ($MSBuildVersion in @("Current", "15.0")) {
                $MSBuildPath = [IO.Path]::Combine($VSInstallDirectory, "MSBuild", $MSBuildVersion, "Bin", "MSBuild.exe")
                if (Test-Path $MSBuildPath) {
                    return $MSBuildPath
                }
            }
        }
    }

    throw "Unable to find MSBuild.exe"
}

function Initialize-Environment() {
    Set-StrictMode -Version Latest
    $ErrorActionPreference="Stop"
    $ProgressPreference="SilentlyContinue"
}

function Clean-Up-Workspace() {
    Say-Invocation $MyInvocation

    if (-Not (Test-Path $ToolsDirectory)) {
        New-Item $ToolsDirectory -ItemType Directory | Out-Null
    }

    Remove-Item -Recurse -Force $OutputDirectory -ErrorAction SilentlyContinue
    New-Item $OutputDirectory -ItemType Directory | Out-Null

    foreach ($ProjectName in $ProjectsToPack) {
        $BinReleaseDirectory = [IO.Path]::Combine($BaseDirectory, "source", $ProjectName, "bin", "Release")
        Remove-Item -Recurse -Force $BinReleaseDirectory -ErrorAction SilentlyContinue
    }

    foreach ($TestProjectName in $TestsToRun) {
        $BinReleaseDirectory = [IO.Path]::Combine($BaseDirectory, "tests", $TestProjectName, "bin", "Release")
        Remove-Item -Recurse -Force $BinReleaseDirectory -ErrorAction SilentlyContinue
    }
}

function Build-Solution() {
    Say-Invocation $MyInvocation

    $MSBuild = Locate-MSBuild

    & $MSBuild "/t:restore" "/v:$MSBuildVerbosity" "/p:Configuration=Release" "/m" $SolutionPath
    & $MSBuild "/t:build" "/v:$MSBuildVerbosity" "/p:Configuration=Release" "/p:TreatWarningsAsErrors=$TreatWarningsAsErrors" "/p:GeneratePackageOnBuild=$BuildNuGet" "/p:MajorI=$MajorI" "/p:MinorI=$MinorI" "/p:BuildI=$BuildI" "/p:MajorIEMDI=$MajorIEMDI" "/p:MinorIEMDI=$MinorIEMDI" "/p:BuildIEMDI=$BuildIEMDI"  "/m" $SolutionPath

    if ($LastExitCode -ne 0) {
        throw "MSBuild failed to build the solution."
    }
}

function Run-Tests() {
    Say-Invocation $MyInvocation

    New-Item $OutputTestResultsDirectory -ItemType Directory | Out-Null

    foreach ($TestProjectName in $TestsToRun) {
        $BinReleaseDirectory = [IO.Path]::Combine($BaseDirectory, "tests", $TestProjectName, "bin", "Release")
        $TestFrameworks = Get-ChildItem $BinReleaseDirectory -Directory | Select -Expand Name

        foreach ($TestFramework in $TestFrameworks) {
            $TestLibraryPath = [IO.Path]::Combine($BinReleaseDirectory, $TestFramework, "$TestProjectName.dll")

            & $VSTestPath $TestLibraryPath /Logger:trx /ResultsDirectory:$OutputTestResultsDirectory
        
            if ($LastExitCode -ne 0) {
                Clean-Up-Workspace
                throw "$TestProjectName ($TestFramework) has detected an error(s)."
            }
        }
    }
}

function Create-Package() {
    Say-Invocation $MyInvocation

    foreach ($ProjectName in $ProjectsToPack) {
        $BinReleaseDirectory = [IO.Path]::Combine($BaseDirectory, "source", $ProjectName, "bin", "Release")
        $LibraryFrameworks = Get-ChildItem $BinReleaseDirectory -Directory | Select -Expand Name

        foreach ($LibraryFramework in $LibraryFrameworks) {
            $ProjectLibraryDirectory = [IO.Path]::Combine($BinReleaseDirectory, $LibraryFramework)

            if (-Not (Test-Path $ProjectLibraryDirectory)) {
                throw "Failed to find $ProjectLibraryDirectory"
            }

            $TargetProjectLibraryDirectory = [IO.Path]::Combine($OutputDirectory, "artifacts", $ProjectName, $LibraryFramework)

            robocopy $ProjectLibraryDirectory $TargetProjectLibraryDirectory *.dll *.pdb *.xml /NFL /NDL /NJS /NC /NS /NP /XO /XF | Out-Null
        }

        if ($BuildNuGet) {
            if (-Not (Test-Path $OutputNuGetDirectory)) {
                New-Item $OutputNuGetDirectory -ItemType Directory | Out-Null
            }

            $NupkgTemplate = [IO.Path]::Combine($BinReleaseDirectory, "*.nupkg")
            $SnupkgTemplate = [IO.Path]::Combine($BinReleaseDirectory, "*.snupkg")

            Copy-Item -Path $NupkgTemplate -Destination $OutputNuGetDirectory
            Copy-Item -Path $SnupkgTemplate -Destination $OutputNuGetDirectory
        }
    }
	
	if ($BuildNuGet) {
		Copy-Item -Path $NuGetPath -Destination $OutputNuGetDirectory
		Copy-Item -Path $NuGetConfigPath -Destination $OutputNuGetDirectory
	}
}

Initialize-Environment
Clean-Up-Workspace
Install-DotNet
Install-NuGet
Install-VSTest
Install-VSWhere
Build-Solution
Run-Tests
Create-Package

Write-Host "`nIntermedium: Done!"
exit 0
