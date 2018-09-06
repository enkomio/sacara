header_VmShiftLeft
vm_shift_left PROC
	push ebp
	mov ebp, esp	
	
	; read the first operand
	push [ebp+arg0]
	call vm_stack_pop
	push eax

	; read the second operand
	push [ebp+arg0]
	call vm_stack_pop
	mov cl, al

	; do operation
	pop eax
	shr eax, cl

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push

	mov ebp, esp
	pop ebp
	ret
vm_shift_left ENDP
header_end