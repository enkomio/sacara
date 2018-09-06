header_VmMul
vm_mul PROC
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
	mul ecx

	; push result
	push eax
	push [ebp+arg0]
	call vm_stack_push

	mov ebp, esp
	pop ebp
	ret
vm_mul ENDP
header_end