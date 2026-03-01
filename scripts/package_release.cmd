@echo off
setlocal enabledelayedexpansion

rem ============================================================
rem package_pluginadmin.cmd
rem
rem Creates a Plugins Admin compatible zip for NppDB:
rem   - Zip root (installed into <...>\plugins\NppDB\): NppDB.dll + *.ini + lib\...
rem   - Zip root MUST contain NppDB.dll as the only DLL file.
rem
rem Usage:
rem   package_pluginadmin.cmd [Configuration] [Platform] [VersionOrTimestamp] [OutputDir]
rem
rem Defaults:
rem   Configuration = Release
rem   Platform      = x64
rem   Version       = <timestamp>   (yyyyMMdd_HHmmss)
rem   OutputDir     = artifacts\pluginadmin
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
if "%OUTDIR%"=="" set "OUTDIR=artifacts\pluginadmin"


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
rem Staging paths
rem ----------------------------
set "STAGE_ROOT=%REPO_ROOT%\%OUTDIR%\NppDB_PluginsAdmin_%VERSION%"
set "STAGE_LIB=%STAGE_ROOT%\lib"

if exist "%STAGE_ROOT%" rmdir /s /q "%STAGE_ROOT%"
mkdir "%STAGE_LIB%" >nul 2>&1


rem ============================================================
rem Copy files into staging
rem ============================================================

rem --- Main plugin DLL (rename NppDB.Plugin.dll -> NppDB.dll)
call :copy_if_exists "%BUILD_OUT%\NppDB.Plugin.dll" "%STAGE_ROOT%\NppDB.dll"

rem --- Project modules (including Comm) go to lib\
call :copy_if_exists "%BUILD_OUT%\NppDB.Comm.dll"       "%STAGE_LIB%\NppDB.Comm.dll"
call :copy_if_exists "%BUILD_OUT%\NppDB.Core.dll"       "%STAGE_LIB%\NppDB.Core.dll"
call :copy_if_exists "%BUILD_OUT%\NppDB.MSAccess.dll"   "%STAGE_LIB%\NppDB.MSAccess.dll"
call :copy_if_exists "%BUILD_OUT%\NppDB.PostgreSQL.dll" "%STAGE_LIB%\NppDB.PostgreSQL.dll"

rem --- Newtonsoft.Json is required at runtime
call :copy_if_exists "%BUILD_OUT%\Newtonsoft.Json.dll" "%STAGE_LIB%\Newtonsoft.Json.dll"

rem --- Runtime dependencies (explicit list to avoid accidental junk)
set "DEPS=Antlr4.Runtime.Standard.dll Npgsql.dll Microsoft.Bcl.AsyncInterfaces.dll Microsoft.Bcl.HashCode.dll Microsoft.Extensions.Logging.Abstractions.dll System.Buffers.dll System.Collections.Immutable.dll System.Diagnostics.DiagnosticSource.dll System.Memory.dll System.Numerics.Vectors.dll System.Runtime.CompilerServices.Unsafe.dll System.Text.Encodings.Web.dll System.Text.Json.dll System.Threading.Channels.dll System.Threading.Tasks.Extensions.dll System.ValueTuple.dll TimeZoneConverter.dll"

for %%F in (%DEPS%) do (
  call :copy_if_exists "%BUILD_OUT%\%%F" "%STAGE_LIB%\%%F"
)

rem --- Translations/config INIs
for %%T in ("%REPO_ROOT%\translations\*.ini") do (
  if exist "%%~fT" copy /y "%%~fT" "%STAGE_ROOT%\" >nul
)


rem ============================================================
rem Validate: only NppDB.dll at zip root
rem ============================================================
set "ROOT_DLLS="
for %%D in ("%STAGE_ROOT%\*.dll") do (
  set "ROOT_DLLS=!ROOT_DLLS! %%~nxD"
)

if not "%ROOT_DLLS%"==" NppDB.dll" (
  echo ERROR: Plugins Admin zip root must contain ONLY NppDB.dll.
  echo Found:%ROOT_DLLS%
  exit /b 3
)


rem ============================================================
rem Zip the staging folder
rem ============================================================
set "ZIP_PATH=%REPO_ROOT%\%OUTDIR%\NppDB_%VERSION%_pluginadmin.zip"

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
