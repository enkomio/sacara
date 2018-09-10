header_VmXor
vm_xor PROC
	push ebp
	mov ebp, esp	
	
	; read the first operand
	push [ebp+arg0]
	call vm_stack_pop_enc
	push eax

	; read the second operand
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov ebx, eax

	; do operation
	pop eax
	xor eax, ebx

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push_enc

	mov ebp, esp
	pop ebp
	ret
vm_xor ENDP
header_end