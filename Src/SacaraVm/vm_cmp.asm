header_VmCmp
vm_cmp PROC
	push ebp
	mov ebp, esp	
	
	; read first argument
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov edx, eax

	; read second argument
	push [ebp+arg0]
	call_vm_stack_pop_enc

	; do comparison and save result
	cmp edx, eax
	pushfd
	pop edx

	; read the current flags
	mov ebx, [ebp+arg0]
	mov ecx, (VmContext PTR [ebx]).flags

	; set overflow flag
	push edx
	popfd
	jno clear_overflow_flag
	or ecx, SET_VM_OVERFLOW_FLAG
	jmp check_zero_flag
clear_overflow_flag:
	and ecx, CLEAR_VM_OVERFLOW_FLAG	

	; set zero flag
check_zero_flag:
	push edx
	popfd
	jnz clear_zero_flag
	or ecx, SET_VM_ZERO_FLAG
	jmp check_carry
clear_zero_flag:
	and ecx, CLEAR_VM_ZERO_FLAG	

	; set carry flag
check_carry:
	push edx
	popfd
	jnc clear_carry_flag
	or ecx, SET_VM_CARRY_FLAG
	jmp check_sign_flag
clear_carry_flag:
	and ecx, CLEAR_VM_CARRY_FLAG
	
	; set sign flag
check_sign_flag:
	push edx
	popfd
	jns clear_sign_flag
	or ecx, SET_VM_SIGN_FLAG
	jmp set_vm_flags
clear_sign_flag:
	and ecx, CLEAR_VM_SIGN_FLAG

set_vm_flags:
	; restore flags
	push edx
	popfd	
	mov (VmContext PTR [ebx]).flags, ecx

	mov esp, ebp
	pop ebp
	ret
vm_cmp ENDP
header_marker