header_VmPushVariable
vm_push_variable PROC
	push ebp
	mov ebp, esp	

	; read the variable index
	push 2
	push [ebp+arg0]
	call vm_read_opcode

	; read the variable value
	push eax
	push [ebp+arg0]
	call vm_local_var_get

	; push the value into the stack
	push eax
	push [ebp+arg0]
	call vm_stack_push
	
	mov ebp, esp
	pop ebp
	ret
vm_push_variable ENDP