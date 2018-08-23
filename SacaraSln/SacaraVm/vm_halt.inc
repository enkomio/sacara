header_VmHalt
vm_halt PROC
	push ebp
	mov ebp, esp
	xor eax, eax
	mov ebp, esp
	pop ebp
	ret
vm_halt ENDP