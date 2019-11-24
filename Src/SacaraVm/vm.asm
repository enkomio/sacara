include typedef.inc
include const.inc
include strings.inc
include instructions_headers.inc
include vm_macro.inc
include utility.asm

; compute size of the code related to the VM. 
; These offset are used by the find_vm_handler routine
start_vm_instructions:
include instructions.inc
VM_INSTRUCTIONS_SIZE DWORD $ - start_vm_instructions

; functions that are exported
include vm_exports.inc

; *****************************
; arguments: vm_context, state
; *****************************
vm_call_error_handler PROC
	push ebp
	mov ebp, esp

	mov eax, [ebp+arg0]
	cmp (VmContext PTR [eax]).error_handler, 0
	je finish

	; invoke the error handler by passing the state and the error code
	push (VmContext PTR [eax]).error_code
	push [ebp+arg1]
	call (VmContext PTR [eax]).error_handler

	; check if the execution needs to halted
	cmp eax, 0
	jne finish

	mov ebx, [ebp+arg0]
	mov eax, (VmContext PTR [ebx]).flags
	or eax, 80000000h ; set halt flag
	mov (VmContext PTR [ebx]).flags, eax
	
finish:
	xor eax, eax
	mov esp, ebp
	pop ebp
	ret 8h
vm_call_error_handler ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_free_stack_frame PROC
	push ebp
	mov ebp, esp

	; get stack pointer addr
	mov eax, [ebp+arg0]
	mov ebx, (VmContext PTR [eax]).stack_frame

	; free vars buffer
	push eax
	push (VmStackFrame PTR [ebx]).locals
	call heap_free
	pop eax

	; free stack frame
	push eax
	push (VmContext PTR [eax]).stack_frame
	call heap_free
	
	mov esp, ebp
	pop ebp
	ret 4h
vm_free_stack_frame ENDP

; *****************************
; arguments: vm context, var index, imm
; *****************************
vm_local_var_set_internal PROC
	push ebp
	mov ebp, esp

	; get the local var buffer	
	mov eax, [ebp+arg0]
	mov eax, (VmContext PTR [eax]).stack_frame
	mov eax, (VmStackFrame PTR [eax]).locals

	; go to the given offset
	mov ebx, [ebp+arg1]
	lea eax, [eax+TYPE DWORD*ebx]

	; set the value
	mov ebx, [ebp+arg2]
	mov [eax], ebx

	mov esp, ebp
	pop ebp
	ret 0Ch
vm_local_var_set_internal ENDP

; *****************************
; arguments: vm context, var index
; *****************************
vm_local_var_get_internal PROC
	push ebp
	mov ebp, esp

	; get the local var buffer
	mov eax, [ebp+arg0]
	mov eax, (VmContext PTR [eax]).stack_frame
	mov eax, (VmStackFrame PTR [eax]).locals

	; go to the given offset
	mov ebx, [ebp+arg1]
	lea eax, [eax+TYPE DWORD*ebx]

	; read the value
	mov eax, [eax]
	mov esp, ebp
	pop ebp
	ret 8h
vm_local_var_get_internal ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_is_stack_empty PROC
	push ebp
	mov ebp, esp

	; get stack pointer addr
	mov ecx, [ebp+arg0]
	mov ecx, (VmContext PTR [ecx]).stack_frame

	mov ebx, (VmStackFrame PTR [ecx]).base
	xor eax, eax
	cmp (VmStackFrame PTR [ecx]).top, ebx	
	jz equals
	jmp finish

equals:
	inc eax
finish:
	mov esp, ebp
	pop ebp
	ret 4h
vm_is_stack_empty ENDP

; *****************************
; arguments: vm_context, increment size
; *****************************
vm_increment_ip PROC
	push ebp
	mov ebp, esp
	mov ecx, [ebp+arg1]
	mov eax, [ebp+arg0]
	mov ebx, [eax]
	lea ebx, [ecx+(VmContext PTR [ebx]).ip]
	mov (VmContext PTR [eax]).ip, ebx
	mov esp, ebp
	pop ebp
	ret 8
vm_increment_ip ENDP

; *****************************
; arguments: vm_context, operand word
; *****************************
vm_decode_word_operand PROC
	push ebp
	mov ebp, esp

	push 0CA50h
	push [ebp+arg1]
	push [ebp+arg0]
	call vm_decode_operand
	and eax, 0FFFFh

	mov esp, ebp
	pop ebp
	ret 8
vm_decode_word_operand ENDP

; *****************************
; arguments: vm_context, operand double word, hardcoded key
; *****************************
vm_decode_operand PROC
	push ebp
	mov ebp, esp

	; check operand encryption flag
	mov eax, [ebp+arg0]
	mov ecx, (VmContext PTR [eax]).flags
	test ecx, 04000000h
	jz not_decode

	; comput dynamic enc key	
	mov ebx, (VmContext PTR [eax]).ip
	mov eax, ebx
	shl ebx, 8h
	or eax, ebx
	shl ebx, 8h
	or eax, ebx
	shl ebx, 8h
	or eax, ebx

	; decrypt operator
	mov ebx, [ebp+arg1]
	mov edx, [ebp+arg2]
	xor ebx, edx
	xor eax, ebx
	jmp finish

not_decode:
	mov eax, [ebp+arg1]

finish:
	mov esp, ebp
	pop ebp
	ret 0Ch
vm_decode_operand ENDP

; *****************************
; arguments: vm_context, extracted opcode
; return: 0 on success, opcode index on error
; *****************************
vm_execute PROC
	push ebp
	mov ebp, esp
	sub esp, 4h

	; find the handler
	push [ebp+arg1]	
	push VM_INSTRUCTIONS_SIZE
	push start_vm_instructions
	call find_vm_handler
	test eax, eax
	je error

	IF ENABLE_CODE_RELOCATION
		; relocate code	
		push eax
		call relocate_code
		test eax, eax
		je error

		; save allocated memory
		mov [ebp+local0], eax 		
	ENDIF

	; invoke the handler
	push [ebp+arg0]
	call eax
	add esp, 04h
	
	xor eax, eax
	jmp end_execution

error:
	; handler not found
	mov eax, [ebp+arg0]
	mov (VmContext PTR [eax]).error_code, VM_HANDLER_NOT_FOUND
	
	; invoke the error handler, passing the offset of the IP generating the errors
	push (VmContext PTR [eax]).ip
	push eax
	call vm_call_error_handler

end_execution:	
	IF ENABLE_CODE_RELOCATION
		push eax
		; free allocated memory
		push [ebp+local0]
		call free_relocated_code
		pop eax
	ENDIF

	mov esp, ebp
	pop ebp
	ret 8
vm_execute ENDP

; *****************************
; arguments: vm_context, opcode
; *****************************
vm_decode_opcode PROC
	push ebp
	mov ebp, esp

	; the first 4 bits of the opcode are flags, 
	; which meaning is: |opcode is encrypted|operand is encrypt|..|..|
		
	; check if the encrypt operands flag is set
	mov eax, [ebp+arg1]
	test eax, 04000h
	jz check_opcode_flags

	; set the operands are encrypted flag in the global status flag
	mov ebx, [ebp+arg0]
	mov ecx, (VmContext PTR [ebx]).flags
	or ecx, 8000000h
	mov (VmContext PTR [ebx]).flags, ecx

check_opcode_flags:
	; check if the encrypt opcode flag is set
	mov eax, [ebp+arg1]
	test eax, 08000h
	jz clear_flags

	; decrypt the opcode
	mov eax, [ebp+arg1]	
	xor eax, INIT_OPCODE_XOR_KEY

	mov ebx, [ebp+arg0]
	xor eax, (VmContext PTR [ebx]).ip

clear_flags:
	; clear first 4 bits since they are flags and save the result
	and eax, 0FFFh

	mov esp, ebp
	pop ebp
	ret 8h
vm_decode_opcode ENDP

; *****************************
; arguments: vm_context, size
; *****************************
vm_read_code PROC
	push ebp
	mov ebp, esp

	; read vm ip
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov ebx, (VmContext PTR [eax]).ip

	; read word opcode
	mov esi, (VmContext PTR [eax]).code
	lea esi, [esi+ebx]
	xor eax, eax

	cmp dword ptr [ebp+arg1], TYPE DWORD
	je read_four_bytes
	mov ax, word ptr [esi]
	jmp finish

read_four_bytes:
	mov eax, dword ptr [esi]

finish:
	push eax
	; increment the VM ip
	push [ebp+arg1]
	push [ebp+arg0]
	call vm_increment_ip
	pop eax

	mov esp, ebp
	pop ebp
	ret 8
vm_read_code ENDP

; *****************************
; arguments: vm_context, operand double word
; *****************************
vm_decode_double_word_operand PROC
	push ebp
	mov ebp, esp

	push 0547431C0h
	push [ebp+arg1]
	push [ebp+arg0]
	call vm_decode_operand	

	mov esp, ebp
	pop ebp
	ret 8
vm_decode_double_word_operand ENDP

; *****************************
; arguments: vm context, imm
; *****************************
vm_stack_push_enc PROC
	push ebp
	mov ebp, esp

	; compute stack offset as XOR key
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	mov ecx, (VmStackFrame PTR [ebx]).top
	sub ecx, (VmStackFrame PTR [ebx]).base
	not ecx
		
	; encode value
	push [ebp+arg1]
	push ecx
	call encode_dword

	; push encoded value
	push eax
	push [ebp+arg0]
	call vm_stack_push

	mov esp, ebp
	pop ebp
	ret 8h
vm_stack_push_enc ENDP

; *****************************
; arguments: vm context
; *****************************
vm_stack_pop_enc PROC
	push ebp
	mov ebp, esp

	; read encoded value
	push [ebp+arg0]
	call vm_stack_pop

	; compute stack offset as XOR key
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	mov ecx, (VmStackFrame PTR [ebx]).top
	sub ecx, (VmStackFrame PTR [ebx]).base
	not ecx

	; decode value
	push eax
	push ecx
	call decode_dword

	mov esp, ebp
	pop ebp
	ret 4h
vm_stack_pop_enc ENDP

; *****************************
; arguments: vm_context, size
; *****************************
vm_clear_operands_encryption_flag PROC
	push ebp
	mov ebp, esp

	mov eax, [ebp+arg0]
	mov ecx, (VmContext PTR [eax]).flags
	and ecx, 0f7ffffffh
	mov (VmContext PTR [eax]).flags, ecx

	mov esp, ebp
	pop ebp
	ret 4
vm_clear_operands_encryption_flag ENDP

; *****************************
; arguments: vm context, imm
; *****************************
vm_stack_push PROC
	push ebp
	mov ebp, esp

	; read stack frame header
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	
	; increment stack by 1
	add (VmStackFrame PTR [ebx]).top, TYPE DWORD
	
	; set value on top of the stack	
	mov eax, [ebp+arg1]
	mov ebx, (VmStackFrame PTR [ebx]).top
	mov [ebx], eax

	mov esp, ebp
	pop ebp
	ret 8h
vm_stack_push ENDP

; *****************************
; arguments: vm context
; *****************************
vm_stack_pop PROC
	push ebp
	mov ebp, esp

	; read stack frame header
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame

	; read value
	mov ecx, (VmStackFrame PTR [ebx]).top
	mov eax, [ecx]
	mov dword ptr [ecx], 0h ; zero the value

	; decrement stack by 1 DWORD
	sub (VmStackFrame PTR [ebx]).top, TYPE DWORD

	mov esp, ebp
	pop ebp
	ret 4h
vm_stack_pop ENDP

; *****************************
; arguments: stack memory, previous stack frame pointer
; *****************************
vm_init_stack_frame PROC
	push ebp
	mov ebp, esp
	
	mov eax, [ebp+arg0] ; read stack base
	mov edx, [ebp+arg1] ; previous stack frame pointer

	; fill stack frame header
	lea ebx, [eax+TYPE DWORD*4 ]
	mov (VmStackFrame PTR [eax]).previous, edx
	mov (VmStackFrame PTR [eax]).base, ebx
	mov (VmStackFrame PTR [eax]).top, ebx

	; init space for local vars	
	mov ebx, [ebp+arg0]
	mov (VmStackFrame PTR [ebx]).locals, 0h
		
	mov esp, ebp
	pop ebp
	ret 8h
vm_init_stack_frame ENDP