header_VmJumpIfGreaterEquals
vm_jump_if_greater_equals PROC
	push ebp
	mov ebp, esp	
	
	; pop the value
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; test the carry flag
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).flags
	test ebx, VM_CARRY_FLAG
	jz modify_ip

	; test the zero flag
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).flags
	test ebx, VM_ZERO_FLAG
	jz finish

modify_ip:
	; modify the vm IP
	mov ebx, [ebp+arg0]
	mov (VmContext PTR [ebx]).ip, eax

finish:	
	mov esp, ebp
	pop ebp
	ret
vm_jump_if_greater_equals ENDP
header_marker