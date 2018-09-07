header_VmAdd
vm_add PROC
	push ebp
	mov ebp, esp	
	
	; read the first operand
	push [ebp+arg0]
	call vm_stack_pop
	push eax

	; read the second operand
	push [ebp+arg0]
	call vm_stack_pop

	; do operation
	pop ecx
	sub ecx, eax

	; push result
	push ecx
	push [ebp+arg0]
	call vm_stack_push

	mov ebp, esp
	pop ebp
	ret
vm_add ENDP
header_end