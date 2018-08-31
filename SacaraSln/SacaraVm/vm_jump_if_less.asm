header_VmJumpIfLess
vm_jump_if_less PROC
	push ebp
	mov ebp, esp	
	
	; pop the value
	push [ebp+arg0]
	call vm_stack_pop

	; read the sign flag
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_flags]
	test ebx, VM_SIGN_FLAG
	jz finish

	; modify the vm IP
	mov ebx, [ebp+arg0]
	mov [ebx+vm_ip], eax

finish:	
	mov ebp, esp
	pop ebp
	ret
vm_jump_if_less ENDP
header_end