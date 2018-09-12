comment !
This is the implementation of the SacaraVM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.CODE

; the flag is specified as a MAM preprocessor.
IF ENABLE_ANTI_ANALYSIS
	include anti_macro.inc
ELSE
	include empty_anti_macro.inc
ENDIF

include vm.asm

main PROC
	push ebp
	mov ebp, esp	
	cmp dword ptr [ebp+arg1], DLL_PROCESS_ATTACH	
	jne finish
	check_debugger
	mov eax, 1		
finish:	
	mov esp, ebp
	pop ebp
	ret
main ENDP
END main

