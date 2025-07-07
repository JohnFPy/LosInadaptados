@echo off
setlocal enabledelayedexpansion

echo =====================================
echo   Verificando Requisitos MoodPress
echo =====================================
echo.

REM Verificar .NET 9
echo [1/3] Verificando .NET SDK...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK no encontrado
    echo Descarga desde: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

for /f "tokens=1" %%a in ('dotnet --version') do set dotnet_version=%%a
echo .NET Version: !dotnet_version!

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