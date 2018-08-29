header_VmPushImmediate
vm_push_immediate PROC
	push ebp
	mov ebp, esp	

	; read the immediate to push	
	push 4
	push [ebp+arg0]
	call vm_read_opcode

	push eax
	push [ebp+arg0]
	call vm_stack_push
	
	mov ebp, esp
	pop ebp
	ret
vm_push_immediate ENDP