header_VmGetIp
vm_get_ip PROC
	push ebp
	mov ebp, esp	
	
	mov eax, [ebp+arg0]
	push [eax+vm_ip]
	push [ebp+arg0]
	call vm_stack_push

	mov ebp, esp
	pop ebp
	ret
vm_get_ip ENDP
header_end