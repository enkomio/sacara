header_VmJumpIfLessEquals
vm_jump_if_less_equals PROC
	push ebp
	mov ebp, esp	
	
	; pop the value
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; read the flags
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).flags
	
	; test relevant flags, see: http://faydoc.tripod.com/cpu/jle.htm
	test ebx, SET_VM_ZERO_FLAG
	jnz modify_ip
	
	xor edx, edx
	or edx, SET_VM_SIGN_FLAG
	or edx, SET_VM_OVERFLOW_FLAG
	and edx, ebx
	popcnt edx, edx
	cmp edx, 1
	jnz finish

modify_ip:
	; modify the vm IP
	mov ebx, [ebp+arg0]
	mov (VmContext PTR [ebx]).ip, eax

finish:	
	mov esp, ebp
	pop ebp
	ret
vm_jump_if_less_equals ENDP
header_marker