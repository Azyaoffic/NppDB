@echo off
setlocal enabledelayedexpansion

rem ============================================================
rem package_release.cmd
rem
rem Creates a distributable zip for NppDB with the manual install layout:
rem   - Zip root (Notepad++ program folder): NppDB.Comm.dll + Newtonsoft.Json.dll
rem   - plugins\NppDB\                          : NppDB.dll + other plugin DLLs + deps + *.ini
rem
rem Usage:
rem   package_release.cmd [Configuration] [Platform] [VersionOrTimestamp] [OutputDir]
rem
rem Defaults:
rem   Configuration = Release
rem   Platform      = x64
rem   Version       = <timestamp>   (yyyyMMdd_HHmmss)
rem   OutputDir     = artifacts\release
rem ============================================================


rem ----------------------------
rem Parse arguments / defaults
rem ----------------------------
set "CONFIGURATION=%~1"
if "%CONFIGURATION%"=="" set "CONFIGURATION=Release"

set "PLATFORM=%~2"
if "%PLATFORM%"=="" set "PLATFORM=x64"

set "VERSION=%~3"
if "%VERSION%"=="" call :make_timestamp VERSION

set "OUTDIR=%~4"
if "%OUTDIR%"=="" set "OUTDIR=artifacts\release"


rem ----------------------------
rem Resolve repo/build paths
rem ----------------------------
set "SCRIPT_DIR=%~dp0"
for %%I in ("%SCRIPT_DIR%..") do set "REPO_ROOT=%%~fI"

set "BUILD_OUT=%REPO_ROOT%\bin\%PLATFORM%\%CONFIGURATION%\NppDB"
if not exist "%BUILD_OUT%" (
  echo ERROR: Build output not found:
  echo   "%BUILD_OUT%"
  echo Build the solution first with:
  echo   Configuration=%CONFIGURATION%  Platform=%PLATFORM%
  exit /b 1
)


rem ----------------------------
rem Staging paths (folder layout mirrors Notepad++)
rem ----------------------------
set "STAGE_ROOT=%REPO_ROOT%\%OUTDIR%\NppDB_%VERSION%"
set "STAGE_PLUGIN=%STAGE_ROOT%\plugins\NppDB"

if exist "%STAGE_ROOT%" rmdir /s /q "%STAGE_ROOT%"
mkdir "%STAGE_PLUGIN%" >nul 2>&1


rem ============================================================
rem Copy files into staging
rem ============================================================

rem --- Files that must sit next to notepad++.exe (zip root)
call :copy_if_exists "%BUILD_OUT%\NppDB.Comm.dll"       "%STAGE_ROOT%\NppDB.Comm.dll"
call :copy_if_exists "%BUILD_OUT%\Newtonsoft.Json.dll" "%STAGE_ROOT%\Newtonsoft.Json.dll"

rem --- Main plugin DLL (rename NppDB.Plugin.dll -> NppDB.dll)
call :copy_if_exists "%BUILD_OUT%\NppDB.Plugin.dll"     "%STAGE_PLUGIN%\NppDB.dll"

rem --- Plugin modules
call :copy_if_exists "%BUILD_OUT%\NppDB.Core.dll"       "%STAGE_PLUGIN%\NppDB.Core.dll"
call :copy_if_exists "%BUILD_OUT%\NppDB.MSAccess.dll"   "%STAGE_PLUGIN%\NppDB.MSAccess.dll"
call :copy_if_exists "%BUILD_OUT%\NppDB.PostgreSQL.dll" "%STAGE_PLUGIN%\NppDB.PostgreSQL.dll"

rem --- Runtime dependencies (explicit list to avoid accidental junk)
set "DEPS=Antlr4.Runtime.Standard.dll Npgsql.dll Microsoft.Bcl.AsyncInterfaces.dll Microsoft.Bcl.HashCode.dll Microsoft.Extensions.Logging.Abstractions.dll System.Buffers.dll System.Collections.Immutable.dll System.Diagnostics.DiagnosticSource.dll System.Memory.dll System.Numerics.Vectors.dll System.Runtime.CompilerServices.Unsafe.dll System.Text.Encodings.Web.dll System.Text.Json.dll System.Threading.Channels.dll System.Threading.Tasks.Extensions.dll System.ValueTuple.dll TimeZoneConverter.dll"

for %%F in (%DEPS%) do (
  call :copy_if_exists "%BUILD_OUT%\%%F" "%STAGE_PLUGIN%\%%F"
)

rem --- Translations/config INIs from build output
for %%T in ("%BUILD_OUT%\*.ini") do (
  if exist "%%~fT" copy /y "%%~fT" "%STAGE_PLUGIN%\" >nul
)

rem --- Optional translations/config INIs from repo translations folder
if exist "%REPO_ROOT%\translations" (
  for %%T in ("%REPO_ROOT%\translations\*.ini") do (
    if exist "%%~fT" copy /y "%%~fT" "%STAGE_PLUGIN%\" >nul
  )
)


rem ============================================================
rem Zip the staging folder
rem ============================================================
set "ZIP_PATH=%REPO_ROOT%\%OUTDIR%\NppDB_%VERSION%.zip"

powershell -NoProfile -Command ^
  "Compress-Archive -Path '%STAGE_ROOT%\*' -DestinationPath '%ZIP_PATH%' -Force" >nul

if errorlevel 1 (
  echo ERROR: Failed to create zip:
  echo   "%ZIP_PATH%"
  exit /b 2
)

echo Created:
echo   "%ZIP_PATH%"
exit /b 0


rem ============================================================
rem Helpers
rem ============================================================

:copy_if_exists
if exist "%~1" (
  copy /y "%~1" "%~2" >nul
) else (
  echo WARNING: Missing "%~1"
)
exit /b 0


:make_timestamp
rem Produces yyyyMMdd_HHmmss from %date% and %time%
rem Assumes date format like dd.MM.yyyy (common in EU locales).
set "ts=%date:~-4%%date:~3,2%%date:~0,2%_%time:~0,2%%time:~3,2%%time:~6,2%"
set "ts=%ts: =0%"
set "%~1=%ts%"
exit /b 0
