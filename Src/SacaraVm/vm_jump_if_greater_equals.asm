header_VmJumpIfGreaterEquals
vm_jump_if_greater_equals PROC
	push ebp
	mov ebp, esp	
	
	; pop the value
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; read the flags
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).flags
	
	; test relevant flags, see: http://faydoc.tripod.com/cpu/jge.htm	
	mov edx, ebx
	mov edi, ebx	
	and edx, SET_VM_SIGN_FLAG
	shr edx, 30
	and edi, SET_VM_OVERFLOW_FLAG
	shr edi, 27
	cmp edi, edx
	jnz finish

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