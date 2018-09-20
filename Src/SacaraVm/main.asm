comment !
This is the implementation of the SacaraVM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096

.DATA

VERSION BYTE '1.1',0

.CODE

; the flag is specified as a MASM preprocessor.
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
	check_debugger_via_trap_flag
	mov eax, 1		
finish:	
	mov esp, ebp
	pop ebp
	ret
main ENDP
END main

