header_VmCall
vm_call PROC
	push ebp
	mov ebp, esp
	sub esp, 0Ch

	; pop the offset to call
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local0], eax

	; pop the number of argument to push in the new stack
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local2], eax

	; allocate space for the stack
	push vm_stack_size
	call_heap_alloc
	mov [ebp+local1], eax

	; init stack frame
	mov ebx, [ebp+arg0]	
	push [ebx+vm_sp] ; previous stack frame
	push eax ; new allocated stack frame
	call_vm_init_stack_frame
		
	; extract the arguments from the stack frame and 
	; temporary save them into the native stack
	mov ecx, [ebp+local2]
get_arguments:	
	push ecx ; save counter
	push [ebp+arg0]
	call_vm_stack_pop_enc
	pop ecx ; restore counter
	push eax ; push argument in the native stack
	loop get_arguments

	; save on top of the current stack frame the ip
	mov eax, [ebp+arg0]	
	push [eax+vm_ip]
	push [ebp+arg0]
	call_vm_stack_push_enc

	; set the new stack frame as the current one
	mov eax, [ebp+local1]
	mov ebx, [ebp+arg0]	
	mov [ebx+vm_sp], eax

	; push the arguments saved in the native 
	; stack in the new managed stack
	mov ecx, [ebp+local2]
set_arguments:
	mov [ebp+local2], ecx ; save counter
	push [ebp+arg0]
	call_vm_stack_push_enc
	mov ecx, [ebp+local2] ; restore counter
	loop set_arguments
		
	; move the sp to the specific offset
	mov ebx, [ebp+local0]
	mov eax, [ebp+arg0]
	mov [eax+vm_ip], ebx
	
	mov esp, ebp
	pop ebp
	ret
vm_call ENDP
header_end