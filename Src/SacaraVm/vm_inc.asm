header_VmInc
vm_inc PROC
	push ebp
	mov ebp, esp	
	
	; push 1
	push 1
	push [ebp+arg0]
	call_vm_stack_push_enc

	; invoke the Add handler
	push [ebp+arg0]
	call vm_add

	mov esp, ebp
	pop ebp
	ret
vm_inc ENDP
header_marker