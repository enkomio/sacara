; *****************************
; arguments: vm context, var index, imm
; return: 
; *****************************
vm_local_var_set PROC PUBLIC vm_context:DWORD, index:DWORD, value:DWORD
	pushad

	; decode vm context
 	call get_vm_dll_base_address
	xor eax, vm_context

	; call the internal function
	push value
	push index
	push eax
	call vm_local_var_set_internal

	popad
	ret 0Ch
vm_local_var_set ENDP

; *****************************
; arguments: vm context, var index
; return: value
; *****************************
vm_local_var_get PROC PUBLIC vm_context:DWORD, index:DWORD
	sub esp, 4
	pushad

	; decode vm context
	call get_vm_dll_base_address
	xor eax, vm_context

	; call the internal function	
	push index
	push eax
	call vm_local_var_get_internal
	mov [ebp+local0], eax

	popad
	mov eax, [ebp+local0]
	ret 8h
vm_local_var_get ENDP

; *****************************
; Initialize a new VM and return an handle to it
; arguments: vm_code, vm_code_size
; return: and handle to the created context
; *****************************
vm_init PROC PUBLIC code:PTR DWORD, code_size:DWORD 
	vm_init_vm_context EQU local0
	pushad
	sub esp, 4

	; allocate space for the VmContext
	push SIZEOF VmContext
	call heap_alloc
	mov [ebp+vm_init_vm_context], eax

	; init the VmContext structure
	mov (VmContext PTR [eax]).ip, dword ptr 0h	; zero VM ip
	mov (VmContext PTR [eax]).flags, dword ptr 0h; zero flags

	; allocate space for the stack
	push VM_STACK_SIZE
	call heap_alloc
	
	; save the stack pointer
	mov ecx, [ebp+vm_init_vm_context]
	mov (VmContext PTR [ecx]).stack_frame, eax

	; init stack frame
	push 0h ; no previous stack frame
	push eax
	call vm_init_stack_frame

	; init the local var space since this is the VM init function
	; by doing so we allow to external program to set local variables
	; value that can be read by the VM code	
	push VM_STACK_VARS_SIZE
	call heap_alloc
	mov ebx, [ebp+vm_init_vm_context]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	mov (VmStackFrame PTR [ebx]).locals, eax
		
	; set the code pointer
	mov ebx, code
	mov ecx, [ebp+vm_init_vm_context]
	mov (VmContext PTR [ecx]).code, ebx

	; set the code size
	mov ebx, code_size
	mov (VmContext PTR [ecx]).code_size, ebx

	check_debugger_via_HeapAlloc

	; return the handle to the VmContext
	call get_vm_dll_base_address
	mov ebx, eax
	mov eax, [ebp+vm_init_vm_context]
	xor eax, ebx
	mov [ebp+vm_init_vm_context], eax
	add esp, 4

	popad
	mov eax, [ebp+vm_init_vm_context]	
	ret 8
vm_init ENDP

; *****************************
; arguments: vm_context
; return: 
; *****************************
vm_free PROC PUBLIC vm_context:PTR DWORD
	pushad

	; decode vm context
	call get_vm_dll_base_address
	xor eax, vm_context

	; push the context to be free later
	push eax

	; free frame	
	push eax
	call vm_free_stack_frame
	
	; free VM context
	call heap_free
	
	popad	
	ret 4h
vm_free ENDP

; *****************************
; arguments: vm_context
; return: 0 on success, opcode index on error
; *****************************
vm_run PROC PUBLIC vm_context:DWORD
	vm_context_local = local1
	
	sub esp, 2 * TYPE DWORD
	pushad

	; decode vm context
	call get_vm_dll_base_address
	xor eax, vm_context
	mov [ebp+vm_context_local], eax

vm_loop:		
	check_debugger_via_trap_flag
	
	; check if we executed all the code
	mov eax, [ebp+vm_context_local]
	mov ebx, (VmContext PTR [eax]).code_size
	cmp ebx, (VmContext PTR [eax]).ip
	je finish

	; read the opcode to execute	
	push 2
	push [ebp+vm_context_local]
	call vm_read_code

	; decode opcode
	push eax
	push [ebp+vm_context_local]
	call vm_decode_opcode

	; execute the VM instruction
	push eax
	push [ebp+vm_context_local]
	call vm_execute
	mov [ebp+local0], eax
		
	; check for generic errors
	test eax, eax
	jne finish

	; check the finish flag in the context
	mov ebx, [ebp+vm_context_local]
	mov ebx, (VmContext PTR [ebx]).flags
	test ebx, 80000000h
	je vm_loop
	
finish:
	mov eax, [ebp+local0]	
	popad
	ret 4h
vm_run ENDP

; *****************************
; arguments: vm_context, error handler function pointer
; *****************************
vm_set_error_handler PROC PUBLIC vm_context:DWORD, handler:PTR DWORD
	pushad 

	; decode vm context
	call get_vm_dll_base_address
	xor eax, vm_context

	; set the error handler in the given VM context	
	mov ebx, handler
	mov (VmContext PTR [eax]).error_handler, ebx

	popad
	ret 8h
vm_set_error_handler ENDP