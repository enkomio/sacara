header_VmCall
vm_call PROC
	push ebp
	mov ebp, esp	

	; allocate space for the stack
	push vm_stack_size
	call heap_alloc
	push eax

	; get current stack frame
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]

	; init stack frame
	push ebx
	push eax
	call vm_init_stack_frame

	; set the new stack frame as the current one
	pop eax
	mov ebx, [ebp+arg0]
	mov [ebx+vm_sp], eax

	; pop the offset to call
	push [ebp+arg0]
	call vm_stack_pop
		
	; move the sp to the specific offset
	mov ebx, [ebp+arg0]
	mov [ebp+vm_ip], eax
	
	mov ebp, esp
	pop ebp
	ret
vm_call ENDP
header_end