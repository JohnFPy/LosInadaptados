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
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9.0" >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: ASP.NET Core Runtime 9.0 no encontrado
    echo Iniciando instalacion de ASP.NET Core Runtime 9.0...
    echo.
    
    REM Detectar arquitectura del sistema
    if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
        set "runtime_installer=dependencies\aspnetcore-runtime-9.0.7-win-x64.exe"
    ) else if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (
        set "runtime_installer=dependencies\aspnetcore-runtime-9.0.7-win-arm64.exe"
    ) else (
        set "runtime_installer=dependencies\aspnetcore-runtime-9.0.7-win-x86.exe"
    )
    
    echo Verificando archivo del instalador: !runtime_installer!
    if not exist "!runtime_installer!" (
        echo ERROR: No se encontro el archivo del instalador
        echo Ruta esperada: !runtime_installer!
        pause
        exit /b 1
    )
    
    echo Instalando ASP.NET Core Runtime 9.0...
    "!runtime_installer!" /quiet /norestart
    
    if %errorlevel% neq 0 (
        echo ERROR: Fallo la instalacion del runtime
        echo Ejecuta manualmente: !runtime_installer!
        pause
        exit /b 1
    )
    
    echo ASP.NET Core Runtime 9.0 instalado exitosamente!
    echo Reiniciando verificacion...
    echo.
)

REM Verificar ASP.NET Core Runtime nuevamente
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9.0" >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: ASP.NET Core Runtime sigue sin estar disponible
    echo Reinicia el sistema y ejecuta este script nuevamente
    pause
    exit /b 1
)

for /f "tokens=2" %%a in ('dotnet --list-runtimes ^| findstr "Microsoft.AspNetCore.App 9.0"') do set runtime_version=%%a
echo ASP.NET Core Runtime Version: !runtime_version!

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