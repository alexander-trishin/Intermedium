[CmdletBinding()]
param(
    [Version] $VersionPrefix = "1.0.1",
    [Version] $AssemblyVersion = "1.0.0.0",
    [Version] $Version = "1.0.1.0",

    [Version] $VSTestVersion = "16.4.0",
    [Version] $VSWhereVersion = "2.3.2",
    [Version] $DotNetVersion = "3.0.100",
    [string] $DotNetChannel = "Current",

    [string] $BaseDirectory = (Resolve-Path ..),
    [string] $BuildDirectory = [IO.Path]::Combine($BaseDirectory, "build"),
    [string] $ToolsDirectory = [IO.Path]::Combine($BuildDirectory, "tools"),
    [string] $VSTestDirectory = [IO.Path]::Combine($ToolsDirectory, "Microsoft.TestPlatform.$VSTestVersion"),
    [string] $VSWhereDirectory = [IO.Path]::Combine($ToolsDirectory, "vswhere.$VSWhereVersion"),
    [string] $OutputDirectory = [IO.Path]::Combine($BuildDirectory, "bin"),
    [string] $OutputNuGetDirectory = [IO.Path]::Combine($OutputDirectory, "nuget"),
    [string] $OutputTestResultsDirectory = [IO.Path]::Combine($OutputDirectory, "test-results"),

    [string] $SolutionPath = [IO.Path]::Combine($BaseDirectory, "Intermedium.sln"),
    [string] $NuGetPath = [IO.Path]::Combine($ToolsDirectory, "nuget.exe"),
    [string] $NuGetConfigPath = [IO.Path]::Combine($BuildDirectory, "nuget.config"),
    [string] $DotNetInstallPath = [IO.Path]::Combine($ToolsDirectory, "dotnet-install.ps1"),

    [Uri] $DotNetInstallUrl = "https://dot.net/v1/dotnet-install.ps1",
    [Uri] $NuGetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe",

    [Array] $Targets = @(
        @{Framework = "netstandard2.0"; TestFramework = "netcoreapp3.0"; Enabled=$true},
        @{Framework = "netstandard1.3"; TestFramework = "netcoreapp2.2"; Enabled=$true},
        @{Framework = "netstandard1.0"; TestFramework = "netcoreapp2.1"; Enabled=$true},
        @{Framework = "net45"; TestFramework = "net46"; Enabled=$true},
        @{Framework = "portable-net45+win8+wpa81+wp8"; TestFramework = "net452"; Enabled=$true}
    ),

    [ValidateSet("q", "quiet", "m", "minimal", "n", "normal", "d", "detailed", "diag", "diagnostic", IgnoreCase = $false)]
    [string] $Verbosity = "m",

    [switch] $TreatWarningsAsErrors = $true,
    [switch] $BuildNuGet = $true
)

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
    Say-Invocation $MyInvocation

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
        & $NuGetPath install $PackageName -Version $PackageVersion -ConfigFile $NuGetConfigPath -OutputDirectory $SaveDirectory
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

function Get-Library-Frameworks() {
    Say-Invocation $MyInvocation

    return ($Targets | Select-Object @{Name="Framework";Expression={$_.Framework}} | select -expand Framework) -join ";"
}

function Get-Test-Frameworks() {
    Say-Invocation $MyInvocation

    return ($Targets | Select-Object @{Name="Resolved";Expression={if ($_.TestFramework -ne $null) { $_.TestFramework } else { $_.Framework }}} | select -expand Resolved) -join ";"
}

function Initialize-Environment() {
    Set-StrictMode -Version Latest
    $ErrorActionPreference="Stop"
    $ProgressPreference="SilentlyContinue"
}

function Validate-Parameters() {
    Say-Invocation $MyInvocation

    Set-Variable -Name "Targets" -Value ($Targets | ? {$_.Enabled}) -Scope Script

    if (!$Targets -or $Targets.Length -eq 0) {
        throw "All targets are disabled"
    }
}

function Clean-Up-Workspace() {
    Say-Invocation $MyInvocation

    if (-Not (Test-Path $ToolsDirectory)) {
        New-Item $ToolsDirectory -ItemType Directory | Out-Null
    }

    if (Test-Path $OutputDirectory) {
        Remove-Item -Recurse -Force $OutputDirectory
    }

    New-Item $OutputDirectory -ItemType Directory | Out-Null
}

function Build-Solution() {
    Say-Invocation $MyInvocation

    $MSBuild = Locate-MSBuild

    $LibraryFrameworks = Get-Library-Frameworks
    $TestFrameworks = Get-Test-Frameworks

    & $MSBuild "/t:restore" "/v:$Verbosity" "/p:Configuration=Release" "/p:LibraryFrameworks=`"$LibraryFrameworks`"" "/p:TestFrameworks=`"$TestFrameworks`"" "/m" $SolutionPath
    & $MSBuild "/t:build" "/v:$Verbosity" "/p:Configuration=Release" "/p:LibraryFrameworks=`"$LibraryFrameworks`"" "/p:TestFrameworks=`"$TestFrameworks`"" "/p:TreatWarningsAsErrors=$TreatWarningsAsErrors" "/p:GeneratePackageOnBuild=$BuildNuGet" "/p:VersionPrefix=$VersionPrefix" "/p:AssemblyVersion=$AssemblyVersion" "/p:FileVersion=$Version" "/m" $SolutionPath

    if ($LastExitCode -ne 0) {
        throw "MSBuild failed to build the solution"
    }
}

function Run-Tests() {
    Say-Invocation $MyInvocation

    New-Item $OutputTestResultsDirectory -ItemType Directory | Out-Null

    foreach ($Target in $Targets) {
        $TestFramework = if ($Target.TestFramework) { $Target.TestFramework } else { $Target.Framework }        
        $TestLibraryPath = [IO.Path]::Combine($BaseDirectory, "tests", "Intermedium.Tests", "bin", "Release", $TestFramework, "Intermedium.Tests.dll")

        & $VSTestPath $TestLibraryPath /Logger:trx /ResultsDirectory:$OutputTestResultsDirectory
        
        if ($LastExitCode -ne 0) {
            Clean-Up-Workspace
            throw "$TestFramework has detected an error(s)"
        }
    }
}

function Create-Package() {
    Say-Invocation $MyInvocation

    $ReleaseDirectory = [IO.Path]::Combine($BaseDirectory, "source", "Intermedium", "bin", "Release")

    foreach ($Target in $Targets) {
        $TargetFrameworkDirectory = [IO.Path]::Combine($ReleaseDirectory, $Target.Framework)

        if (-Not (Test-Path $TargetFrameworkDirectory)) {
            throw "Failed to find $TargetFrameworkDirectory"
        }

        $TargetOutputDirectory = [IO.Path]::Combine($OutputDirectory, $Target.Framework)

        robocopy $TargetFrameworkDirectory $TargetOutputDirectory *.dll *.pdb *.xml /NFL /NDL /NJS /NC /NS /NP /XO /XF | Out-Null
    }

    if ($BuildNuGet) {
        New-Item $OutputNuGetDirectory -ItemType Directory | Out-Null
        
        $NupkgTemplate = [IO.Path]::Combine($ReleaseDirectory, "*.nupkg")
        $SnupkgTemplate = [IO.Path]::Combine($ReleaseDirectory, "*.snupkg")

        Copy-Item -Path $NupkgTemplate -Destination $OutputNuGetDirectory
        Copy-Item -Path $SnupkgTemplate -Destination $OutputNuGetDirectory
    }
}

Initialize-Environment
Validate-Parameters
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
