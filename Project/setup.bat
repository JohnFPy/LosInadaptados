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
echo   Ejecutando MoodPress
echo =====================================
echo.
echo Ejecutandose con permisos de administrador...
echo.

REM Detectar arquitectura del sistema y ejecutar el executable correspondiente
if "%PROCESSOR_ARCHITECTURE%"=="AMD64" (
    set "project_exe=releases\x64\Project.exe"
) else if "%PROCESSOR_ARCHITECTURE%"=="ARM64" (
    set "project_exe=releases\arm64\Project.exe"
) else (
    set "project_exe=releases\x86\Project.exe"
)

echo Ejecutando proyecto desde: !project_exe!
"!project_exe!"

pause