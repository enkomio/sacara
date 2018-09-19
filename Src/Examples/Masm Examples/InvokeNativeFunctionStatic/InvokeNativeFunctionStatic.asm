.686
.MMX
.XMM
.model flat,stdcall
option casemap:none
include \masm32\macros\macros.asm

include InvokeNativeFunctionStatic.inc

.code

start:

    Invoke GetModuleHandle, NULL
    mov hInstance, eax
    Invoke GetCommandLine
    mov CommandLine, eax
    Invoke InitCommonControls
    mov icc.dwSize, sizeof INITCOMMONCONTROLSEX
    mov icc.dwICC, ICC_COOL_CLASSES or ICC_STANDARD_CLASSES or ICC_WIN95_CLASSES
    Invoke InitCommonControlsEx, Offset icc
    
    Invoke WinMain, hInstance, NULL, CommandLine, SW_SHOWDEFAULT
    Invoke ExitProcess, eax

;------------------------------------------------------------------------------
; WinMain
;------------------------------------------------------------------------------
WinMain PROC hInst:HINSTANCE, hPrevInst:HINSTANCE, CmdLine:LPSTR, CmdShow:DWORD
    LOCAL wc:WNDCLASSEX
    LOCAL msg:MSG

    mov wc.cbSize, SIZEOF WNDCLASSEX
    mov wc.style, CS_HREDRAW or CS_VREDRAW
    mov wc.lpfnWndProc, Offset WndProc
    mov wc.cbClsExtra, NULL
    mov wc.cbWndExtra, DLGWINDOWEXTRA
    push hInst
    pop wc.hInstance
    mov wc.hbrBackground, COLOR_BTNFACE+1 ; COLOR_WINDOW+1
    mov wc.lpszMenuName, IDM_MENU
    mov wc.lpszClassName, Offset ClassName
    Invoke LoadIcon, NULL, IDI_APPLICATION
    mov  wc.hIcon, eax
    mov wc.hIconSm, eax
    Invoke LoadCursor, NULL, IDC_ARROW
    mov wc.hCursor,eax
    Invoke RegisterClassEx, Addr wc
    Invoke CreateDialogParam, hInstance, IDD_DIALOG, NULL, Addr WndProc, NULL
    mov hWnd, eax
    Invoke ShowWindow, hWnd, SW_SHOWNORMAL
    Invoke UpdateWindow, hWnd
    .WHILE TRUE
        Invoke GetMessage, Addr msg, NULL, 0, 0
        .BREAK .if !eax
        Invoke TranslateMessage, Addr msg
        Invoke DispatchMessage, Addr msg
    .ENDW
    mov eax, msg.wParam
    ret
WinMain ENDP


;------------------------------------------------------------------------------
; WndProc - Main Window Message Loop
;------------------------------------------------------------------------------
WndProc PROC hWin:HWND, uMsg:UINT, wParam:WPARAM, lParam:LPARAM
    
    mov eax, uMsg
    .IF eax == WM_INITDIALOG
        ; Init Stuff Here
 
    .ELSEIF eax == WM_COMMAND
        mov eax, wParam
        and eax, 0FFFFh
        .IF eax == IDM_FILE_EXIT
            Invoke SendMessage, hWin, WM_CLOSE, 0, 0
            
        .ELSEIF eax == IDM_HELP_ABOUT
            Invoke ShellAbout, hWin, Addr AppName, Addr AboutMsg,NULL
            
        .ELSEIF eax == IDC_CLICKME
            Invoke DoVMStuff            
            
        .ENDIF

    .ELSEIF eax == WM_CLOSE
        Invoke DestroyWindow, hWin
        
    .ELSEIF eax == WM_DESTROY
        Invoke PostQuitMessage, NULL
        
    .ELSE
        Invoke DefWindowProc, hWin, uMsg, wParam, lParam
        ret
    .ENDIF
    xor eax, eax
    ret
WndProc ENDP


;------------------------------------------------------------------------------
; SacaraVm example using statically linked vm functions - See Notes.txt
;------------------------------------------------------------------------------
DoVMStuff PROC
    LOCAL CTX:VM_CONTEXT
    LOCAL result:DWORD
    LOCAL ptrHelloWorld:DWORD
    LOCAL ptrVMCODE:DWORD
    LOCAL ptrCTX:DWORD
    
    ; Zero out CTX (or use RtlZeroMemory to do it)
    mov CTX.ptrIp, 0
    mov CTX.ptrStack,0
    mov CTX.dwStatus,0
    mov CTX.ptrCode,0
    mov CTX.dwCodeSize,0

    ; initialize the VM context structure
    Invoke vm_init, Addr CTX, Addr VMCODE, VMCODE_SIZE

    ; add as local var the function address
    Invoke vm_local_var_set, Addr CTX, 0, Addr hello_world

    ; run the code
    Invoke vm_run, Addr CTX
    mov result, eax

    ; free the VM
    Invoke vm_free, Addr CTX

    mov eax, result
    ret

DoVMStuff ENDP


;------------------------------------------------------------------------------
; Function to call from vm code
;------------------------------------------------------------------------------
hello_world PROC
    
    Invoke MessageBox, 0, Addr szHelloWorld, Addr AppName, MB_OK
    ret

hello_world ENDP

end start
