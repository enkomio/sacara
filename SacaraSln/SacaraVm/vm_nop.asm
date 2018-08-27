header_VmNop
vm_nop PROC
	push ebp
	mov ebp, esp	
	mov eax, eax
	mov ebp, esp
	pop ebp
	ret
vm_nop ENDP