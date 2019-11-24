comment !
This is the implementation of the SacaraVM.
2018 (C) Antonio 's4tan' Parata
!

.686
.model flat, stdcall
.stack 4096

.DATA

VERSION BYTE '2.0',0

.CODE

include build_options.inc
include anti_macro.inc
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

