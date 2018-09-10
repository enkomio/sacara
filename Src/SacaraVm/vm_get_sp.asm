header_VmGetSp
vm_get_sp PROC
	push ebp
	mov ebp, esp	
	
	mov eax, [ebp+arg0]
	push [eax+vm_sp]
	push [ebp+arg0]
	call vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_get_sp ENDP
header_end