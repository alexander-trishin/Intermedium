cls
powershell -ExecutionPolicy RemoteSigned -File %~dp0build.ps1

if not ["%errorlevel%"]==["0"] (
    pause
    exit /b %errorlevel%
)
