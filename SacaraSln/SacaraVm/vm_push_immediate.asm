header_VmPushImmediate
vm_push_immediate PROC
	push ebp
	mov ebp, esp	

	; read the immediate to push	
	push 4
	push [ebp+arg0]
	call vm_read_opcode

	; read stack frame header
	mov ebx, [ebp+arg0]
	mov ebx, [ebx+vm_sp] 
	
	; set value to top of the stack
	mov ecx, [ebx+vm_stack_top]
	mov [ecx], eax

	; increment stack by 1
	lea ecx, [ecx+TYPE DWORD]
	mov [ebx+vm_stack_top], ecx
	
	mov ebp, esp
	pop ebp
	ret
vm_push_immediate ENDP