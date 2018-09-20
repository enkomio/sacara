include const.inc
include strings.inc
include instructions_headers.inc
include utility.asm

include vm_macro.inc

; compute size of the code related to the VM. 
; These offset are used by the find_vm_handler routine
start_vm_instructions:
include instructions.inc
vm_instructions_size DWORD $ - start_vm_instructions

; *****************************
; arguments: vm_context, size
; *****************************
vm_read_code PROC
	push ebp
	mov ebp, esp

	; read vm ip
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov ebx, [eax+vm_ip]

	; read word opcode
	mov esi, [eax+vm_code]
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
	mov ebx, [ebx+vm_sp]
	mov ecx, [ebx+vm_stack_top]
	sub ecx, [ebx+vm_stack_base]
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
	mov ebx, [ebx+vm_sp]
	mov ecx, [ebx+vm_stack_top]
	sub ecx, [ebx+vm_stack_base]
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
	mov ecx, [eax+vm_flags]
	and ecx, 0f7ffffffh
	mov [eax+vm_flags], ecx

	mov esp, ebp
	pop ebp
	ret 4
vm_clear_operands_encryption_flag ENDP

; *****************************
; arguments: vm context, var index, imm
; *****************************
vm_local_var_set PROC
	push ebp
	mov ebp, esp
	pushad

	; get the local var buffer
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]
	mov eax, [eax+vm_local_vars]

	; go to the given offset
	mov ebx, [ebp+arg1]
	lea eax, [eax+TYPE DWORD*ebx]

	; set the value
	mov ebx, [ebp+arg2]
	mov [eax], ebx

	popad
	mov esp, ebp
	pop ebp
	ret 0Ch
vm_local_var_set ENDP

; *****************************
; arguments: vm context, var index
; *****************************
vm_local_var_get PROC
	push ebp
	mov ebp, esp
	sub esp, 4
	pushad

	; get the local var buffer
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]
	mov eax, [eax+vm_local_vars]

	; go to the given offset
	mov ebx, [ebp+arg1]
	lea eax, [eax+TYPE DWORD*ebx]

	; read the value
	mov eax, [eax]
	mov [ebp+local0], eax

	popad
	mov eax, [ebp+local0]
	mov esp, ebp
	pop ebp
	ret 8h
vm_local_var_get ENDP


; *****************************
; arguments: vm context, imm
; *****************************
vm_stack_push PROC
	push ebp
	mov ebp, esp

	; read stack frame header
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp] 
	
	; increment stack by 1
	add dword ptr [ebx+vm_stack_top], TYPE DWORD
	
	; set value on top of the stack	
	mov eax, [ebp+arg1]
	mov ebx, [ebx+vm_stack_top]
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
	mov ebx, [ebx+vm_sp]

	; read value
	mov ecx, [ebx+vm_stack_top]
	mov eax, [ecx]
	mov dword ptr [ecx], 0h ; zero the value

	; decrement stack by 1 DWORD
	sub dword ptr [ebx+vm_stack_top], TYPE DWORD

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
	mov dword ptr [eax+vm_stack_previous_frame], edx
	mov [eax+vm_stack_base], ebx
	mov [eax+vm_stack_top], ebx

	; init space for local vars	
	mov ebx, [ebp+arg0]
	mov dword ptr [ebx+vm_local_vars], 0h
		
	mov esp, ebp
	pop ebp
	ret 8h
vm_init_stack_frame ENDP


; *****************************
; arguments: vm_context, vm_code, vm_code_size
; *****************************
vm_init PROC
	push ebp
	mov ebp, esp
	pushad 

	mov eax, [ebp+arg0]
	mov [eax+vm_ip], dword ptr 0h	; zero VM ip
	mov [eax+vm_flags], dword ptr 0h; zero flags

	; allocate space for the stack
	push vm_stack_size
	call heap_alloc
	
	; save the stack pointer
	mov ecx, [ebp+arg0]
	mov [ecx+vm_sp], eax

	; init stack frame
	push 0h ; no previous stack frame
	push eax
	call vm_init_stack_frame

	; init the local var space since this is the VM init function
	; by doing so we allow to external program to set local variables
	; value that can be read by the VM code	
	push vm_stack_vars_size
	call heap_alloc
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]
	mov [ebx+vm_local_vars], eax
		
	; set the code pointer
	mov ebx, [ebp+arg1]
	mov ecx, [ebp+arg0]
	mov [ecx+vm_code], ebx

	; set the code size
	mov ebx, [ebp+arg2]
	mov [ecx+vm_code_size], ebx

	check_debugger_via_HeapAlloc

	popad
	mov esp, ebp
	pop ebp
	ret 0Ch
vm_init ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_free PROC
	push ebp
	mov ebp, esp
	pushad

	; get stack pointer addr
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]

	; free vars buffer
	push [eax+vm_local_vars]
	call heap_free

	; free stack frame	
	mov eax, [ebp+arg0]
	push [eax+vm_sp]
	call heap_free
	
	popad
	mov esp, ebp
	pop ebp
	ret 4h
vm_free ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_is_stack_empty PROC
	push ebp
	mov ebp, esp

	; get stack pointer addr
	mov ecx, [ebp+arg0]
	mov ecx, [ecx+vm_sp]

	mov ebx, [ecx+vm_stack_base]
	xor eax, eax
	cmp [ecx+vm_stack_top], ebx	
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
	lea ebx, [ebx+vm_ip+ecx]
	mov [eax+vm_ip], ebx
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
	mov ecx, [eax+vm_flags]
	test ecx, 08000000h
	jz not_decode

	; comput dynamic enc key	
	mov ebx, [eax+vm_ip]
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
	push vm_instructions_size
	push start_vm_instructions
	call find_vm_handler
	test eax, eax
	je error

	; relocate code	
	push eax
	call relocate_code
	test eax, eax
	je error

	; save allocated memory
	mov [ebp+local0], eax 

	; invoke the handler
	push [ebp+arg0]			; VM context	
	call eax
	add esp, 04h
	
	xor eax, eax
	jmp end_execution

error:
	; invalid opcode, set the halt flag and error flag
	mov eax, [ebp+arg0]
	mov ebx, [eax+vm_flags]
	or ebx, 0C0000000h
	mov [eax+vm_flags], ebx

	; set eax to the offset of the opcode that generated the error
	mov eax, [eax+vm_ip]

end_execution:	
	push eax
	; free allocated memory
	push [ebp+local0]
	call free_relocated_code
	pop eax

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
	mov ecx, [ebx+vm_flags]
	or ecx, 8000000h
	mov [ebx+vm_flags], ecx

check_opcode_flags:
	; check if the encrypt opcode flag is set
	mov eax, [ebp+arg1]
	test eax, 08000h
	jz clear_flags

	; decrypt the opcode
	mov eax, [ebp+arg1]	
	xor eax, INIT_OPCODE_XOR_KEY

	mov ebx, [ebp+arg0]
	xor eax, [ebx+vm_ip]

clear_flags:
	; clear first 4 bits since they are flags and save the result
	and eax, 0FFFh

	mov esp, ebp
	pop ebp
	ret 8h
vm_decode_opcode ENDP

; *****************************
; arguments: vm_context
; return: 0 on success, opcode index on error
; *****************************
vm_run PROC
	push ebp
	mov ebp, esp
	sub esp, 4
	pushad

vm_loop:		
	check_debugger_via_trap_flag

	; read the opcode to execute	
	push 2
	push [ebp+arg0]
	call vm_read_code

	; decode opcode
	push eax
	push [ebp+arg0]
	call vm_decode_opcode

	; execute the VM instruction
	push eax
	push [ebp+arg0]
	call vm_execute
	mov [ebp+local0], eax
		
	; check the finish flag in the context
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_flags]
	test ebx, 80000000h
	je vm_loop
	
	popad
	mov eax, [ebp+local0]
	mov esp, ebp
	pop ebp
	ret 8
vm_run ENDP