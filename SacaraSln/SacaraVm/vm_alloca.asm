header_VmAlloca
vm_alloca PROC
	push ebp
	mov ebp, esp	
	
	; free previous allocated space
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]
	push [eax+vm_local_vars]
	call heap_free
	
	; read the immediate	
	push 4
	push [ebp+arg0]
	call vm_read_opcode

	; allocate the new memory
	lea eax, [eax*TYPE DWORD]
	push eax
	call heap_alloc

	; set the new memory in the header
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp]
	mov [ebx+vm_local_vars], eax
	
	mov ebp, esp
	pop ebp
	ret
vm_alloca ENDP
header_end