header_VmJump
vm_jump PROC
	push ebp
	mov ebp, esp	
	
	; read the offset to jump
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; modify the vm IP
	mov ebx, [ebp+arg0]
	mov (VmContext PTR [ebx]).ip, eax

	mov esp, ebp
	pop ebp
	ret
vm_jump ENDP
header_marker