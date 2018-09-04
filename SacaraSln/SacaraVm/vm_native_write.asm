header_VmNativeWrite
vm_native_write PROC
	push ebp
	mov ebp, esp	
	sub esp, 4
	
	; read the native address
	push [ebp+arg0]
	call vm_stack_pop
	mov [ebp+local0], eax

	; read byte to write
	push [ebp+arg0]
	call vm_stack_pop
			
	; write the byte
	mov ecx, [ebp+local0]
	mov [ebx], cl
	
	add esp, 4
	mov ebp, esp
	pop ebp
	ret
vm_native_write ENDP
header_end