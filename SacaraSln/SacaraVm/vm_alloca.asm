header_VmAlloca
vm_alloca PROC
	push ebp
	mov ebp, esp
		
	; read the first argument which is how many DWORD to allocate
	push [ebp+arg0]
	call vm_read_dword_argument
	
	; TODO *****************************
	; increase stack space for current function

	; read and update vm stack pointer
	mov ebx, [ebp+arg0]
	mov esi, [ebx+vm_sp]
	lea esi, [esi+eax*4]
	mov [ebx+vm_sp], esi
	
	; increment vm ip
	push 4
	push ebx
	call vm_increment_ip

	mov ebp, esp
	pop ebp
	ret
vm_alloca ENDP