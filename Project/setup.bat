@echo off

REM Auto-elevate to administrator privileges
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
    echo Solicitando permisos de administrador...
    goto UACPrompt
) else ( goto gotAdmin )

:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    echo UAC.ShellExecute "%~s0", "", "", "runas", 1 >> "%temp%\getadmin.vbs"
    "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
    if exist "%temp%\getadmin.vbs" ( del "%temp%\getadmin.vbs" )
    pushd "%CD%"
    CD /D "%~dp0"

setlocal enabledelayedexpansion

echo =====================================
echo   Verificando Requisitos MoodPress
echo =====================================
echo.
echo Ejecutandose con permisos de administrador...
echo.

REM Verificar .NET 9
echo [1/3] Verificando .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: .NET SDK no encontrado
    echo Iniciando descarga automatica de .NET SDK 9.0...
    echo.
    
    REM Detectar arquitectura del sistema
    if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
        set "sdk_url=https://download.visualstudio.microsoft.com/download/pr/4e766615-57e6-4b1d-a574-25eeb7a71107/9306af6accef6bf21de8a59f78c8868b/dotnet-sdk-9.0.100-win-x64.exe"
        set "installer_name=dotnet-sdk-9.0.100-win-x64.exe"
    ) else if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (
        set "sdk_url=https://download.visualstudio.microsoft.com/download/pr/4e766615-57e6-4b1d-a574-25eeb7a71107/b4a9b96a32b83bbbaed5a5776e93894e/dotnet-sdk-9.0.100-win-arm64.exe"
        set "installer_name=dotnet-sdk-9.0.100-win-arm64.exe"
    ) else (
        set "sdk_url=https://download.visualstudio.microsoft.com/download/pr/4e766615-57e6-4b1d-a574-25eeb7a71107/8b3b312142e6e3a0b42fd4c4a8e64c31/dotnet-sdk-9.0.100-win-x86.exe"
        set "installer_name=dotnet-sdk-9.0.100-win-x86.exe"
    )
    
    echo Descargando !installer_name!...
    powershell -Command "& {[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri '!sdk_url!' -OutFile '!installer_name!' -UseBasicParsing}"
    
    if %errorlevel% neq 0 (
        echo ERROR: Fallo la descarga del SDK
        echo Descarga manualmente desde: https://dotnet.microsoft.com/download/dotnet/9.0
        pause
        exit /b 1
    )
    
    echo Instalando .NET SDK 9.0...
    "!installer_name!" /quiet /norestart
    
    if %errorlevel% neq 0 (
        echo ERROR: Fallo la instalacion del SDK
        echo Ejecuta manualmente: !installer_name!
        pause
        exit /b 1
    )
    
    echo Limpiando archivos temporales...
    del "!installer_name!"
    
    echo .NET SDK 9.0 instalado exitosamente!
    echo Reiniciando verificacion...
    echo.
)

REM Verificar version de .NET nuevamente
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK sigue sin estar disponible
    echo Reinicia el sistema y ejecuta este script nuevamente
    pause
    exit /b 1
)

for /f "tokens=1" %%a in ('dotnet --version') do set dotnet_version=%%a
echo .NET Version: !dotnet_version!

REM Verificar que sea .NET 9.x
echo !dotnet_version! | findstr /r "^9\." >nul
if %errorlevel% neq 0 (
    echo WARNING: Se encontro .NET version !dotnet_version!, pero se requiere .NET 9.x
    echo Puede que necesites reinstalar o actualizar .NET SDK
    pause
)

echo [2/3] Restaurando dependencias...
cd src
dotnet restore
if %errorlevel% neq 0 (
    echo ERROR: Fallo al restaurar paquetes
    pause
    exit /b 1
)

echo [3/3] Ejecutando proyecto...
echo.
dotnet run

pause