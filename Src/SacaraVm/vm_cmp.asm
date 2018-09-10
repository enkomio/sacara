header_VmCmp
vm_cmp PROC
	push ebp
	mov ebp, esp	
	
	; read arguments
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov edx, eax

	push [ebp+arg0]
	call vm_stack_pop_enc

	; do comparison and save result
	cmp edx, eax
	pushfd

	; read the current flags
	mov ebx, [ebp+arg0]
	mov ecx, [ebx+vm_flags]

	; check zero flag
	jz set_zero_flag
	and ecx, 0DFFFFFFFh
	jmp check_carry_flag

set_zero_flag:
	or ecx, 020000000h

check_carry_flag:
	; restore flags
	popfd

	jc set_carry_flag
	and ecx, 0EFFFFFFFh
	jmp finish

set_carry_flag:
	or ecx, 010000000h

finish:
	mov [ebx+vm_flags], ecx

	mov ebp, esp
	pop ebp
	ret
vm_cmp ENDP
header_end