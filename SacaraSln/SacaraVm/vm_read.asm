header_VmRead
vm_read PROC
	push ebp
	mov ebp, esp	
	
	; pop the offset that we want to read
	push [ebp+arg0]
	call vm_stack_pop
	
	; read opcode
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_code]
	add eax, ebx
	movzx eax, byte ptr [eax]
	
	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call vm_stack_push
	
	mov ebp, esp
	pop ebp
	ret
vm_read ENDP
header_end