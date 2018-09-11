header_VmNativeRead
vm_native_read PROC
	push ebp
	mov ebp, esp	
	
	; pop the address that we want to read from native memory
	push [ebp+arg0]
	call_vm_stack_pop_enc
	
	; read byte
	movzx eax, byte ptr [eax]
	
	; push the value on top of the stack
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc
	
	mov esp, ebp
	pop ebp
	ret
vm_native_read ENDP
header_end