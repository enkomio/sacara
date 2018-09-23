header_VmWrite
vm_write PROC
	push ebp
	mov ebp, esp	
	sub esp, 8
	
	; read the offset
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local0], eax

	; read opcode to write
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local1], eax

	; to go the offset
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).code
	add ebx, [ebp+local0]
	
	; write the byte
	mov ecx, [ebp+local1]
	mov [ebx], cl
	
	mov esp, ebp
	pop ebp
	ret
vm_write ENDP
header_marker