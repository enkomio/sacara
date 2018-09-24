header_VmGetSp
vm_get_sp PROC
	push ebp
	mov ebp, esp	
	
	mov eax, [ebp+arg0]
	mov eax, (VmContext PTR [eax]).stack_frame
	push (VmStackFrame PTR [eax]).base
	push [ebp+arg0]
	call_vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_get_sp ENDP
header_marker