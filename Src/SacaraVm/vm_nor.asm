header_VmNor
vm_nor PROC
	push ebp
	mov ebp, esp	
	
	; read the first operand
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; read the second operand
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; do NOR operation
	pop ebx
	xchg eax, ebx
	or eax, ebx
	not eax
	
	; push result
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_nor ENDP
header_marker