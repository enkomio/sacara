header_VmPop
vm_pop PROC
	push ebp
	mov ebp, esp	

	sub esp, 4

	; get the value to insert in the local var
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov [ebp+local0], eax

	; read the local variable index
	push 2
	push [ebp+arg0]
	call vm_read_code

	; set the local var
	push [ebp+local0]
	push eax	
	push [ebp+arg0]
	call vm_local_var_set
	
	add esp, 4
	mov ebp, esp
	pop ebp
	ret
vm_pop ENDP
header_end