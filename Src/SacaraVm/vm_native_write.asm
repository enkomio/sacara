header_VmNativeWrite
vm_native_write PROC
	push ebp
	mov ebp, esp
		
	; read the native address
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; read value to write
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; read type
	push [ebp+arg0]
	call_vm_stack_pop_enc
	pop ecx
	pop edi
	
	; write the value according to type
	cmp eax, 3
	jz dword_type
	cmp eax, 2	
	jz word_type
	mov byte ptr [edi], cl
	jmp exit
word_type:
	mov word ptr [edi], cx
	jmp exit
dword_type:
	mov dword ptr [edi], ecx

exit:
	mov esp, ebp
	pop ebp
	ret
vm_native_write ENDP
header_marker