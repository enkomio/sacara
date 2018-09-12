header_VmNop
vm_nop PROC
	push ebp
	mov ebp, esp	
	mov eax, eax
	mov esp, ebp
	pop ebp
	ret
vm_nop ENDP
header_marker