header_VmJumpImmediate
vm_jmp_immediate PROC
	push ebp
	mov ebp, esp	
	mov eax, eax
	mov ebp, esp
	pop ebp
	ret
vm_jmp_immediate ENDP
header_end