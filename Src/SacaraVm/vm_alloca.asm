header_VmAlloca
vm_alloca PROC
	push ebp
	mov ebp, esp

	check_debugger
	
	; pop the value to alloca
	push [ebp+arg0]
	call_vm_stack_pop_enc
	push eax

	; verify if not already allocated
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]
	cmp dword ptr [eax+vm_local_vars], 0h
	pop eax
	jnz finish	

	; allocate the new memory
	lea eax, [eax*TYPE DWORD]
	push eax
	call_heap_alloc

	; set the new memory in the header
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]
	mov [ebx+vm_local_vars], eax
	
finish:
	mov esp, ebp
	pop ebp
	ret
vm_alloca ENDP
header_end