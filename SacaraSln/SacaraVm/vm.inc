include vm_halt.inc
include vm_nop.inc

; arguments: vm_context
vm_init PROC
	push ebp
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov [eax+vm_ip], dword ptr 0h	; zero VM ip
	mov [eax+vm_sp], dword ptr 0h	; zero VM sp
	mov [eax+vm_flags], dword ptr 0h; zero flags
	mov ebp, esp
	pop ebp
	ret
vm_init ENDP

; arguments: vm_context
vm_increment_ip PROC
	push ebp
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov ebx, [eax]
	lea ebx, [ebx+vm_ip+2]
	mov [eax+vm_ip], ebx
	mov ebp, esp
	pop ebp
	ret 4
vm_increment_ip ENDP

; arguments: vm_context, code_ptr
vm_read_opcode PROC
	push ebp
	; read vm ip
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov eax, [eax]

	; read word opcode
	mov esi, [ebp+arg1]
	lea esi, [esi+eax]
	mov eax, [esi]

	mov ebp, esp
	pop ebp
	ret 8
vm_read_opcode ENDP

; arguments: vm_context, opcode
vm_execute PROC
	push ebp
	mov ebp, esp			
	mov eax, [ebp+arg1] ; read the VM opcode
	
	;TODOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO

	cmp ax, 7453h

	; end of execution
	xor eax, eax

	mov ebp, esp
	pop ebp
	ret
vm_execute ENDP

; arguments: vm_context, code_ptr

vm_main PROC	
	push ebp
	mov ebp, esp

	sub esp, 8	
	mov eax, [ebp+arg0]
	mov [ebp+vm_ctx], eax
	mov eax, [ebp+arg1]
	mov [ebp+vm_code], eax
	
vm_loop:		
	push [ebp+vm_code]
	push [ebp+vm_ctx]
	call vm_read_opcode
	push eax

	push [ebp+vm_ctx]
	call vm_increment_ip

	push [ebp+vm_ctx]
	call vm_execute	

	test eax, eax ; test if the HALT command was executed
	jne vm_loop

	mov ebp, esp
	pop ebp
	ret
vm_main ENDP