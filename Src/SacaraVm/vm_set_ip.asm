header_VmSetIp
vm_set_ip PROC
	push ebp
	mov ebp, esp	

	; read the argument
	push [ebp+arg0]
	call_vm_stack_pop_enc
	
	; set the IP
	mov ebx, [ebp+arg0]
	mov (VmContext PTR [ebx]).ip, eax

	mov esp, ebp
	pop ebp
	ret
vm_set_ip ENDP
header_marker