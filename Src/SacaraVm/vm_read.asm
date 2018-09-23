header_VmRead
vm_read PROC
	push ebp
	mov ebp, esp	
	
	; pop the offset that we want to read
	push [ebp+arg0]
	call_vm_stack_pop_enc
	
	; read opcode
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).code
	add eax, ebx
	movzx eax, byte ptr [eax]
	
	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc
	
	mov esp, ebp
	pop ebp
	ret
vm_read ENDP
header_marker