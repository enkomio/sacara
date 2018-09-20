header_VmSetSp
vm_set_sp PROC
	push ebp
	mov ebp, esp	

	; read the argument
	push [ebp+arg0]
	call_vm_stack_pop_enc
	
	; set the Stack Pointer
	mov ebx, [ebp+arg0]
	mov [ebx+vm_sp], eax

	mov esp, ebp
	pop ebp
	ret
vm_set_sp ENDP
header_marker