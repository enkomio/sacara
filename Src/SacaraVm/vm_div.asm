header_VmDiv
vm_div PROC
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
	xor edx, edx
	div ebx

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_div ENDP
header_end