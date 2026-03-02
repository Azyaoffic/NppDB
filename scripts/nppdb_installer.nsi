; NppDB simple NSIS installer (x64-only)
; Payload next to this .nsi:
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
!include "nsDialogs.nsh"

!define PRODUCT_NAME "NppDB"
!define PRODUCT_VERSION "1.0.0"
!define COMPANY_NAME "NppDB"
!define REGROOT HKLM
!define REGKEY "Software\${COMPANY_NAME}\${PRODUCT_NAME}"

Name "${PRODUCT_NAME} Plugin for Notepad++"
OutFile "NppDB_Setup_x64.exe"

Var NPPROOT
Var DLG
Var TXT_PATH
Var BTN_BROWSE

!define MUI_ABORTWARNING
Page custom SelectNppDirPage SelectNppDirLeave
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

; -------- Helpers: safe Notepad++ install dir autodetect ----------
Function DetectNppInstallDir
  StrCpy $0 ""

  ClearErrors
  ReadRegStr $0 HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  IfErrors +2 0
  Goto found

  StrCpy $0 ""
  ClearErrors
  ReadRegStr $0 HKLM "Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  IfErrors +2 0
  Goto found

  StrCpy $0 ""
  ClearErrors
  ReadRegStr $0 HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\Notepad++" "InstallLocation"
  IfErrors done 0
  Goto found

found:
  IfFileExists "$0\notepad++.exe" 0 done
  StrCpy $NPPROOT $0

done:
FunctionEnd

Function .onInit
  ${IfNot} ${RunningX64}
    MessageBox MB_ICONSTOP "This installer is x64-only."
    Abort
  ${EndIf}

  ; Default if detection fails
  StrCpy $NPPROOT "$PROGRAMFILES64\Notepad++"
  Call DetectNppInstallDir
FunctionEnd

; -------- Custom page: select Notepad++ root ----------
Function SelectNppDirPage
  nsDialogs::Create 1018
  Pop $DLG
  StrCmp $DLG error 0 +2
  Abort

  ; Title / instructions
  ${NSD_CreateLabel} 0 0 100% 24u "Select your Notepad++ installation folder (the one containing notepad++.exe)."
  Pop $0

  ; Textbox with detected/default path
  ${NSD_CreateText} 0 28u 78% 12u "$NPPROOT"
  Pop $TXT_PATH

  ; Browse button
  ${NSD_CreateButton} 80% 28u 20% 12u "Browse..."
  Pop $BTN_BROWSE
  ${NSD_OnClick} $BTN_BROWSE OnBrowseNpp

  nsDialogs::Show
FunctionEnd

Function OnBrowseNpp
  ${NSD_GetText} $TXT_PATH $0
  nsDialogs::SelectFolderDialog "Select Notepad++ folder" $0
  Pop $1
  StrCmp $1 "" done
  ${NSD_SetText} $TXT_PATH $1
done:
FunctionEnd

Function SelectNppDirLeave
  ${NSD_GetText} $TXT_PATH $0
  GetFullPathName $0 "$0"

  ; Allow selecting:
  ; - Notepad++ root (contains notepad++.exe)
  ; - ...\plugins
  ; - ...\plugins\NppDB
  StrCpy $NPPROOT $0
  IfFileExists "$NPPROOT\notepad++.exe" ok

  GetFullPathName $NPPROOT "$0\.."
  IfFileExists "$NPPROOT\notepad++.exe" ok

  GetFullPathName $NPPROOT "$0\..\.."
  IfFileExists "$NPPROOT\notepad++.exe" ok

  MessageBox MB_ICONSTOP "notepad++.exe not found in:$\r$\n$0$\r$\n$\r$\nSelect the Notepad++ installation folder (contains notepad++.exe), or its plugins folder."
  Abort

ok:
FunctionEnd

; -------- Optional: block install/uninstall if Notepad++ is running ----------
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

  ; Install into the selected Notepad++ root
  StrCpy $1 "$NPPROOT\plugins\NppDB"

  CreateDirectory "$1"
  CreateDirectory "$1\lib"

  SetOutPath "$1"
  File "installer_payload\NppDB.dll"
  File /nonfatal "installer_payload\english.ini"
  File /nonfatal "installer_payload\estonian.ini"

  SetOutPath "$1\lib"
  File /r "installer_payload\lib\*.dll"

  WriteUninstaller "$1\Uninstall.exe"

  WriteRegStr ${REGROOT} "${REGKEY}" "NppDir" "$NPPROOT"
  WriteRegStr ${REGROOT} "${REGKEY}" "PluginDir" "$1"
  WriteRegStr ${REGROOT} "${REGKEY}" "Version" "${PRODUCT_VERSION}"
SectionEnd

Section "Uninstall"
  Call un.CheckNotepadPP

  ReadRegStr $0 ${REGROOT} "${REGKEY}" "PluginDir"
  StrCmp $0 "" 0 haveDir
  StrCpy $0 "$PROGRAMFILES64\Notepad++\plugins\NppDB"
haveDir:

  RMDir /r "$0"
  DeleteRegKey ${REGROOT} "${REGKEY}"
SectionEnd