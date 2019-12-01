header_VmNativeRead
vm_native_read PROC
	push ebp
	mov ebp, esp	
	
	; pop the address that we want to read from native memory
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; pop the offset type that we want to read
	push [ebp+arg0]
	call_vm_stack_pop_enc
	pop esi

	; read the value according to type
	cmp eax, 3
	jz dword_type
	cmp eax, 2	
	jz word_type
	movzx eax, byte ptr [esi]
	jmp push_value
word_type:
	movzx eax, word ptr [esi]
	jmp push_value
dword_type:
	mov eax, dword ptr [esi]
		
push_value:
	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc
	
	mov esp, ebp
	pop ebp
	ret
vm_native_read ENDP
header_marker