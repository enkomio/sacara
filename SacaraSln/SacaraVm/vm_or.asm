header_VmOr
vm_or PROC
	push ebp
	mov ebp, esp	
	
	; read the first operand
	push [ebp+arg0]
	call vm_stack_pop
	push eax

	; read the second operand
	push [ebp+arg0]
	call vm_stack_pop
	mov ebx, eax

	; do operation
	pop eax
	or eax, ebx

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push

	mov ebp, esp
	pop ebp
	ret
vm_or ENDP
header_end