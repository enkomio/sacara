include windows.inc
include user32.inc
include kernel32.inc
include gdi32.inc
include shell32.inc
include comctl32.inc
include comdlg32.inc
include masm32.inc

includelib user32.lib
includelib kernel32.lib
includelib gdi32.lib
includelib shell32.lib
includelib comctl32.lib
includelib comdlg32.lib
includelib masm32.lib

include SacaraVm.inc    ; Include file for exported vm functions
includelib SacaraVm.lib ; DLL import library for SacaraVm.dll
                        ; Requires SacaraVm.dll to be in folder or on search path

;------------------------------------------------------------------------------
; InvokeNativeFunctionDll Prototypes
;------------------------------------------------------------------------------
WinMain                 PROTO :DWORD,:DWORD,:DWORD,:DWORD
WndProc                 PROTO :DWORD,:DWORD,:DWORD,:DWORD
DoVMStuff               PROTO
hello_world             PROTO


VM_CONTEXT              STRUCT
    ptrIp               DWORD ?
    ptrStack            DWORD ?
    dwStatus            DWORD ?
    ptrCode             DWORD ?
    dwCodeSize          DWORD ?
VM_CONTEXT              ENDS



.CONST
;------------------------------------------------------------------------------
; InvokeNativeFunctionDll Constants
;------------------------------------------------------------------------------
; Main Dialog
IDD_DIALOG              EQU 1000
IDC_CLICKME             EQU 1001

; Main Menu 
IDM_MENU                EQU 10000
IDM_FILE_EXIT           EQU 10001
IDM_HELP_ABOUT          EQU 10101



.DATA
;------------------------------------------------------------------------------
; InvokeNativeFunctionDll Initialized Data
;------------------------------------------------------------------------------
ClassName               DB 'DLGCLASS',0
AppName                 DB 'SacaraVm Dll Test',0
AboutMsg                DB 'fearless 2018',0
szHelloWorld            DB "Hello from the Matrix!!",0

; this code was generated with the Sacara assembler
VMCODE                  DB 0CFh,04Eh,0C7h,037h,072h,052h    ; CF4EC7377252 */ loc_00000000: VmPushImmediate 0x527237C7
	                    DB 0C7h,00Fh                        ; C70F         */ loc_00000006: VmAlloca 
	                    DB 0CFh,04Eh,0CEh,03Fh,07Ah,05Ah    ; CF4ECE3F7A5A */ loc_00000008: VmPushImmediate 0x5A7A3FCE
	                    DB 0CFh,04Eh,0D4h,025h,060h,040h    ; CF4ED4256040 */ loc_0000000E: VmPushImmediate 0x406025D4
	                    DB 0FDh,04Dh,048h,0D2h              ; FD4D48D2     */ loc_00000014: VmPushVariable 0xD248
	                    DB 0A0h,002h                        ; A002         */ loc_00000018: VmNativeCall 
	                    DB 0ECh,003h                        ; EC03         */ loc_0000001A: VmHalt 

VMCODE_SIZE             DD $-VMCODE



.DATA?
;------------------------------------------------------------------------------
; InvokeNativeFunctionDll Uninitialized Data
;------------------------------------------------------------------------------
icc                     INITCOMMONCONTROLSEX <>
hInstance               DD ?
CommandLine             DD ?
hWnd                    DD ?





