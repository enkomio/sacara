header_VmJumpIfGreat
vm_jump_if_great PROC
	push ebp
	mov ebp, esp	
	
	; pop the value
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; read the carry flag
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_flags]
	test ebx, VM_CARRY_FLAG
	jnz finish

	; modify the vm IP
	mov ebx, [ebp+arg0]
	mov [ebx+vm_ip], eax

finish:	
	mov esp, ebp
	pop ebp
	ret
vm_jump_if_great ENDP
header_marker