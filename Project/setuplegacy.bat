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

REM Verificar ASP.NET Core Runtime
echo [1/3] Verificando ASP.NET Core Runtime...
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App" >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: ASP.NET Core Runtime no encontrado
    echo Iniciando descarga automatica de ASP.NET Core Runtime 9.0.7...
    echo.
    
    REM Detectar arquitectura del sistema
    if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
        set "runtime_url=https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/9.0.7/aspnetcore-runtime-9.0.7-win-x64.exe"
        set "installer_name=aspnetcore-runtime-9.0.7-win-x64.exe"
    ) else if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (
        set "runtime_url=https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/9.0.7/aspnetcore-runtime-9.0.7-win-arm64.exe"
        set "installer_name=aspnetcore-runtime-9.0.7-win-arm64.exe"
    ) else (
        set "runtime_url=https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/9.0.7/aspnetcore-runtime-9.0.7-win-x86.exe"
        set "installer_name=aspnetcore-runtime-9.0.7-win-x86.exe"
    )
    
    echo Descargando !installer_name!...
    powershell -Command "& {[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri '!runtime_url!' -OutFile '!installer_name!' -UseBasicParsing}"
    
    if %errorlevel% neq 0 (
        echo ERROR: Fallo la descarga del ASP.NET Core Runtime
        echo Descarga manualmente desde: https://dotnet.microsoft.com/download/dotnet/9.0
        pause
        exit /b 1
    )
    
    echo Instalando ASP.NET Core Runtime 9.0.7...
    "!installer_name!" /quiet /norestart
    
    if %errorlevel% neq 0 (
        echo ERROR: Fallo la instalacion del ASP.NET Core Runtime
        echo Ejecuta manualmente: !installer_name!
        pause
        exit /b 1
    )
    
    echo Limpiando archivos temporales...
    del "!installer_name!"
    
    echo ASP.NET Core Runtime 9.0.7 instalado exitosamente!
    echo Reiniciando verificacion...
    echo.
)

REM Verificar version de ASP.NET Core Runtime nuevamente
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App" >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: ASP.NET Core Runtime sigue sin estar disponible
    echo Reinicia el sistema y ejecuta este script nuevamente
    pause
    exit /b 1
)

REM Mostrar version del runtime encontrado
echo ASP.NET Core Runtime instalado:
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App"

REM Verificar que sea version 9.0.7 o compatible
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App" | findstr "9.0.7" >nul
if %errorlevel% neq 0 (
    echo WARNING: No se encontro ASP.NET Core Runtime version 9.0.7 especificamente
    echo Versiones disponibles:
    dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App"
    echo.
    echo Puede que necesites la version exacta 9.0.7 para el proyecto
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