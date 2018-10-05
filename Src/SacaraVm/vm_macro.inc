call_vm_read_code MACRO
	mov eax, vm_read_code
	call eax
ENDM

call_vm_decode_double_word_operand MACRO
	mov eax, vm_decode_double_word_operand
	call eax
ENDM

call_vm_decode_word_operand MACRO
	mov eax, vm_decode_word_operand
	call eax
ENDM

call_vm_local_var_set MACRO
	mov eax, vm_local_var_set_internal
	call eax
ENDM

call_vm_local_var_get MACRO
	mov eax, vm_local_var_get_internal
	call eax
ENDM

call_vm_free_frame MACRO
	mov eax, vm_free_frame
	call eax
ENDM

call_vm_stack_push_enc MACRO
	mov eax, vm_stack_push_enc
	call eax
ENDM

call_vm_is_stack_empty MACRO
	mov eax, vm_is_stack_empty
	call eax
ENDM

call_vm_stack_pop_enc MACRO
	mov eax, vm_stack_pop_enc
	call eax
ENDM

call_vm_clear_operands_encryption_flag MACRO
	mov eax, vm_clear_operands_encryption_flag
	call eax
ENDM

call_heap_alloc MACRO
	mov eax, heap_alloc
	call eax
ENDM

call_vm_init_stack_frame MACRO
	mov eax, vm_init_stack_frame
	call eax
ENDM