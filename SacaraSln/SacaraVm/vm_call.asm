header_VmCall
vm_call PROC
	push ebp
	mov ebp, esp	

	sub esp, 8

	; pop the offset to call
	push [ebp+arg0]
	call vm_stack_pop
	mov [ebp+local0], eax

	; save on top of the current stack frame the ip
	mov eax, [ebp+arg0]	
	push [eax+vm_ip]
	push [ebp+arg0]
	call vm_stack_push

	; allocate space for the stack
	push vm_stack_size
	call heap_alloc
	mov [ebp+local1], eax

	; init stack frame
	mov ebx, [ebp+arg0]	
	push [ebx+vm_sp] ; previous stack frame
	push eax ; new allocated stack frame
	call vm_init_stack_frame

	; set the new stack frame as the current one
	mov eax, [ebp+local1]
	mov ebx, [ebp+arg0]
	mov [ebx+vm_sp], eax
		
	; move the sp to the specific offset
	mov ebx, [ebp+local0]
	mov eax, [ebp+arg0]
	mov [eax+vm_ip], ebx
	
	add esp, 8
	mov ebp, esp
	pop ebp
	ret
vm_call ENDP
header_end