header_VmPushImmediate
vm_push_immediate PROC
	push ebp
	mov ebp, esp
	check_debugger_via_trap_flag

	; read the immediate to push	
	push 4
	push [ebp+arg0]
	call_vm_read_code

	; decode operand if necessary
	push eax
	push [ebp+arg0]
	call_vm_decode_double_word_operand

	; push the value
	push eax
	push [ebp+arg0]
	call_vm_stack_push_enc

	; clear operands encrypted flag
	push [ebp+arg0]
	call_vm_clear_operands_encryption_flag
	
	mov esp, ebp
	pop ebp
	ret
vm_push_immediate ENDP
header_marker