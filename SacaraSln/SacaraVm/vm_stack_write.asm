header_VmStackWrite
vm_stack_write PROC
	push ebp
	mov ebp, esp
	sub esp, 4
	
	; pop the offset that we want to write to
	push [ebp+arg0]
	call vm_stack_pop
	mov [ebp+local0], eax

	; pop the value that we want to write
	push [ebp+arg0]
	call vm_stack_pop

	; read the stack base
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]
	mov ebx, [ebx+vm_stack_base]

	; go to the offset and write the value
	mov ecx, [ebp+local0]
	lea ebx, [ebx+TYPE DWORD*ecx]	
	mov [ebx], eax
		
	add esp, 4
	mov ebp, esp
	pop ebp
	ret
vm_stack_write ENDP