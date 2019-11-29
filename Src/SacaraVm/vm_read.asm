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
	add ebx, eax
	push ebx

	; pop the offset type that we want to read
	push [ebp+arg0]
	call_vm_stack_pop_enc
	pop ebx
	
	; read the value according to type
	cmp eax, 3
	jz dword_type
	cmp eax, 2	
	jz word_type
	movzx eax, byte ptr [ebx]
	jmp push_value
word_type:
	movzx eax, word ptr [ebx]
	jmp push_value
dword_type:
	mov eax, dword ptr [ebx]

push_value:
	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc
	
	mov esp, ebp
	pop ebp
	ret
vm_read ENDP
header_marker