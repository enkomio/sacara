header_VmStackWrite
vm_stack_write PROC
	push ebp
	mov ebp, esp
	sub esp, 4
	
	; pop the offset that we want to write to
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local0], eax

	; pop the value that we want to write
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; read the stack base
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	mov ebx, (VmStackFrame PTR [ebx]).base

	; go to the offset and write the value
	mov ecx, [ebp+local0]
	lea ebx, [ebx+TYPE DWORD*ecx]	
	mov [ebx], eax
		
	mov esp, ebp
	pop ebp
	ret
vm_stack_write ENDP
header_marker