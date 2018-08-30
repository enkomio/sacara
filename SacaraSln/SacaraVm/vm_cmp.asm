header_VmCmp
vm_cmp PROC
	push ebp
	mov ebp, esp	
	
	; read arguments
	push [ebp+arg0]
	call vm_stack_pop
	mov edx, eax

	push [ebp+arg0]
	call vm_stack_pop

	; read the current flags
	mov ebx, [ebp+arg0]
	mov ecx, [ebx+vm_flags]

	cmp edx, eax

	; check zero flag
	jz set_zero_flag
	and ecx, 0DFFFFFFFh
	jmp check_sign_flag

set_zero_flag:
	or ecx, 020000000h

check_sign_flag:
	cmp edx, eax
	js set_sign_flag
	and ecx, 0EFFFFFFFh
	jmp finish

set_sign_flag:
	or ecx, 010000000h

finish:
	mov [ebx+vm_flags], ecx

	mov ebp, esp
	pop ebp
	ret
vm_cmp ENDP
header_end