@echo off
setlocal ENABLEDELAYEDEXPANSION

set "DOTNET_URL=https://download.visualstudio.microsoft.com/download/pr/1b144bc4-fc3c-4cfd-81d5-4c6934141447/d65f0d9c4e9b8e78d6ac65e1a7207d30/dotnet-sdk-8.0.100-win-x64.exe"
set "DOTNET_INSTALLER=%TEMP%\dotnet-sdk-installer.exe"

echo [🔍] Verificando si .NET SDK está instalado...
where dotnet >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo [❌] .NET SDK no encontrado.
    echo [✅ ] Descargando .NET SDK 8.0...
    
    powershell -Command "Invoke-WebRequest -Uri '%DOTNET_URL%' -OutFile '%DOTNET_INSTALLER%'"
    
    IF EXIST "%DOTNET_INSTALLER%" (
        echo [✅ ] Ejecutando instalador en modo silencioso...
        "%DOTNET_INSTALLER%" /quiet /norestart

        echo [✅] Esperando a que la instalación finalice...
        timeout /t 30 >nul

        echo [✅] Eliminando instalador...
        del "%DOTNET_INSTALLER%"
    ) ELSE (
        echo [❌] Error al descargar el instalador de .NET.
        pause
        exit /b
    )
)

where dotnet >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo [❌] .NET SDK no se detecta incluso después de la instalación.
    echo [✅] Reinicia el sistema y vuelve a ejecutar este archivo.
    pause
    exit /b
)

echo [✅] .NET SDK detectado.
echo.

echo [✅] Restaurando dependencias...
dotnet restore

echo.
echo [✅] Compilando la solución...
dotnet build --no-restore

echo.
echo [✅] Ejecutando pruebas...
dotnet test --no-build --logger "console;verbosity=detailed"

echo.
echo [✅] Proceso completado.
pause
