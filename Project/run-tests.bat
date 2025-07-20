
@echo off
setlocal ENABLEDELAYEDEXPANSION

:: Variables de configuración
set "DOTNET_VERSION=9.0.303"
set "DOTNET_ZIP_URL=https://builds.dotnet.microsoft.com/dotnet/Sdk/%DOTNET_VERSION%/dotnet-sdk-%DOTNET_VERSION%-win-x64.zip"
set "DOTNET_ZIP=%TEMP%\dotnet-sdk.zip"
set "DOTNET_DIR=%~dp0dotnet"
set "DOTNET_EXE=%DOTNET_DIR%\dotnet.exe"

echo ------------------------------------------
echo [✅] Ejecutando entorno de pruebas .NET
echo ------------------------------------------

:: Verifica si el dotnet portable ya está extraído
if not exist "%DOTNET_EXE%" (
    echo [✅ ] Descargando .NET SDK %DOTNET_VERSION%...
    powershell -Command "Invoke-WebRequest -Uri '%DOTNET_ZIP_URL%' -OutFile '%DOTNET_ZIP%'"

    echo [✅] Descomprimiendo SDK en: %DOTNET_DIR%
    powershell -Command "Expand-Archive -Path '%DOTNET_ZIP%' -DestinationPath '%DOTNET_DIR%' -Force"

    del "%DOTNET_ZIP%"
) else (
    echo [✅] SDK ya descargado y extraído.
)

:: Exporta variables para entorno portable
set "DOTNET_ROOT=%DOTNET_DIR%"
set "PATH=%DOTNET_DIR%;%PATH%"

:: Mostrar versión activa
echo.
"%DOTNET_EXE%" --version

echo.
echo [✅] Restaurando dependencias...
"%DOTNET_EXE%" restore

echo.
echo [✅] Compilando proyecto de pruebas...
"%DOTNET_EXE%" build --no-restore

echo.
echo [✅] Ejecutando tests...
"%DOTNET_EXE%" test --no-build --logger "console;verbosity=detailed"

echo.
echo [✅] Proceso de pruebas finalizado.
pause
