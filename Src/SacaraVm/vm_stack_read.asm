header_VmStackRead
vm_stack_read PROC
	push ebp
	mov ebp, esp	
	
	; pop the offset that we want to read
	push [ebp+arg0]
	call vm_stack_pop_enc

	; read the stack base
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]
	mov ebx, [ebx+vm_stack_base]

	; go to the offset and read the value
	lea ebx, [ebx+TYPE DWORD*eax]
	mov eax, [ebx]

	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call vm_stack_push_enc
	
	mov esp, ebp
	pop ebp
	ret
vm_stack_read ENDP
header_end