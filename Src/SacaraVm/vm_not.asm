header_VmNot
vm_not PROC
	push ebp
	mov ebp, esp	
	
	; read the operand
	push [ebp+arg0]
	call vm_stack_pop_enc

	; do operation	
	not eax

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_not ENDP
header_end