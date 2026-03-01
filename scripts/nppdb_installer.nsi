; NppDB simple NSIS installer (x64-only, installs into Notepad++\plugins\NppDB\*)
; Put your built plugin payload next to this .nsi file under:
;   installer_payload\
;     NppDB.dll
;     english.ini
;     estonian.ini
;     lib\*.dll
;
; Build:
;   "C:\Program Files (x86)\NSIS\Bin\makensis.exe" .\nppdb_installer.nsi

Unicode True
RequestExecutionLevel admin

!include "MUI2.nsh"
!include "x64.nsh"

!define PRODUCT_NAME "NppDB"
!define PRODUCT_VERSION "1.0.0"
!define COMPANY_NAME "NppDB"
!define REGROOT HKLM
!define REGKEY "Software\${COMPANY_NAME}\${PRODUCT_NAME}"

Name "${PRODUCT_NAME} Plugin for Notepad++"
OutFile "NppDB_Setup_x64.exe"

; User selects Notepad++ root folder containing notepad++.exe
InstallDir "$PROGRAMFILES64\Notepad++"

!define MUI_ABORTWARNING
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

; -------- Helpers: detect Notepad++ install dir ----------
Function DetectNppInstallDir
  ; Default stays as InstallDir.
  ; Try common uninstall registry keys (order matters).
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  StrCmp $0 "" 0 found
  ReadRegStr $0 HKLM "Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  StrCmp $0 "" 0 found
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  StrCmp $0 "" done found
found:
  StrCpy $INSTDIR $0
done:
FunctionEnd

Function .onInit
  ${IfNot} ${RunningX64}
    MessageBox MB_ICONSTOP "This installer is x64-only."
    Abort
  ${EndIf}

  Call DetectNppInstallDir
FunctionEnd

; Validate selected directory (must be Notepad++ root)
Function .onVerifyInstDir
  IfFileExists "$INSTDIR\notepad++.exe" ok
  MessageBox MB_ICONSTOP "notepad++.exe not found in:$\r$\n$INSTDIR$\r$\n$\r$\nSelect your Notepad++ installation folder (the one containing notepad++.exe)."
  Abort
ok:
FunctionEnd

; -------- Optional: refuse install while Notepad++ is running ----------
Function CheckNotepadPP
  FindWindow $0 "Notepad++" ""
  StrCmp $0 0 done
  MessageBox MB_ICONEXCLAMATION|MB_OKCANCEL \
    "Notepad++ appears to be running. Close it before installing/removing plugins.$\r$\n$\r$\nPress OK once closed, or Cancel to abort." \
    IDOK retry IDCANCEL cancel
retry:
  FindWindow $0 "Notepad++" ""
  StrCmp $0 0 done
  Goto retry
cancel:
  Abort
done:
FunctionEnd

Function un.CheckNotepadPP
  FindWindow $0 "Notepad++" ""
  StrCmp $0 0 done
  MessageBox MB_ICONEXCLAMATION|MB_OKCANCEL \
    "Notepad++ appears to be running. Close it before installing/removing plugins.$\r$\n$\r$\nPress OK once closed, or Cancel to abort." \
    IDOK retry IDCANCEL cancel
retry:
  FindWindow $0 "Notepad++" ""
  StrCmp $0 0 done
  Goto retry
cancel:
  Abort
done:
FunctionEnd

Section "Install"
  Call CheckNotepadPP

  ; Plugin folder inside Notepad++
  StrCpy $1 "$INSTDIR\plugins\NppDB"

  CreateDirectory "$1"
  CreateDirectory "$1\lib"

  ; Copy main plugin + ini to plugin root
  SetOutPath "$1"
  File "installer_payload\NppDB.dll"
  File /nonfatal "installer_payload\english.ini"
  File /nonfatal "installer_payload\estonian.ini"

  ; Copy deps to lib
  SetOutPath "$1\lib"
  File /r "installer_payload\lib\*.dll"

  ; Write uninstaller in plugin folder
  WriteUninstaller "$1\Uninstall.exe"

  ; Store install dir for uninstall
  WriteRegStr ${REGROOT} "${REGKEY}" "NppDir" "$INSTDIR"
  WriteRegStr ${REGROOT} "${REGKEY}" "PluginDir" "$1"
  WriteRegStr ${REGROOT} "${REGKEY}" "Version" "${PRODUCT_VERSION}"
SectionEnd

Section "Uninstall"
  Call un.CheckNotepadPP

  ; Try to read where we installed
  ReadRegStr $0 ${REGROOT} "${REGKEY}" "PluginDir"
  StrCmp $0 "" 0 haveDir
  StrCpy $0 "$PROGRAMFILES64\Notepad++\plugins\NppDB"
haveDir:

  RMDir /r "$0"
  DeleteRegKey ${REGROOT} "${REGKEY}"
SectionEnd