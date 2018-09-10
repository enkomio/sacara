comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.CODE

include vm.asm

main PROC
	push ebp
	mov ebp, esp
	
	cmp dword ptr [ebp+arg1], DLL_PROCESS_ATTACH	
	jne finish
	mov eax, 1
		
finish:

	mov esp, ebp
	pop ebp
	ret
main ENDP
END main

