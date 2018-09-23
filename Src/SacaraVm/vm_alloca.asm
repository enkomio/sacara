header_VmAlloca
vm_alloca PROC
	push ebp
	mov ebp, esp
	check_debugger_via_trap_flag
	
	; pop the value to alloca
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; verify if not already allocated
	mov eax, [ebp+arg0]
	mov eax, (VmContext PTR [eax]).stack_frame
	cmp (VmStackFrame PTR [eax]).locals, 0h
	pop eax
	jnz finish	

	; allocate the new memory
	lea eax, [eax*TYPE DWORD]
	push eax
	call_heap_alloc

	; set the new memory in the header
	mov ebx, [ebp+arg0]
	mov ebx, (VmContext PTR [ebx]).stack_frame
	mov (VmStackFrame PTR [ebx]).locals, eax
	
finish:
	mov esp, ebp
	pop ebp
	ret
vm_alloca ENDP
header_marker