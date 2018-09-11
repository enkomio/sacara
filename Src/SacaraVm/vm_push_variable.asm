header_VmPushVariable
vm_push_variable PROC
	push ebp
	mov ebp, esp	

	; read the variable index
	push 2
	push [ebp+arg0]
	call_vm_read_code

	; decode operand if necessary
	push eax
	push [ebp+arg0]
	call_vm_decode_word_operand

	; read the variable value
	push eax
	push [ebp+arg0]
	call_vm_local_var_get

	; push the value into the stack
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc

	; clear operands encrypted flag
	push [ebp+arg0]
	call_vm_clear_operands_encryption_flag
	
	mov esp, ebp
	pop ebp
	ret
vm_push_variable ENDP
header_end