; *****************************
; arguments: vm_context, vm_code, vm_code_size
; *****************************
vm_init PROC
	push ebp
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov [eax+vm_ip], dword ptr 0h	; zero VM ip
	mov [eax+vm_flags], dword ptr 0h; zero flags
	
	; TODO *****************************
	; allocate vm stack space
	mov [eax+vm_sp], dword ptr 0h	; zero VM sp
	
	; set the code pointer
	mov ebx, [ebp+arg1]
	mov [eax+vm_code], ebx

	; set the code size
	mov ebx, [ebp+arg2]
	mov [eax+vm_code_size], ebx

	mov ebp, esp
	pop ebp
	ret 0Ch
vm_init ENDP

; *****************************
; arguments: vm_context, increment size
; *****************************
vm_increment_ip PROC
	push ebp
	mov ebp, esp
	mov ecx, [ebp+arg1]
	mov eax, [ebp+arg0]
	mov ebx, [eax]
	lea ebx, [ebx+vm_ip+ecx]
	mov [eax+vm_ip], ebx
	mov ebp, esp
	pop ebp
	ret 8
vm_increment_ip ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_read_opcode PROC
	push ebp
	mov ebp, esp

	; read vm ip
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov ebx, [eax+vm_ip]

	; read word opcode
	mov esi, [eax+vm_code]
	lea esi, [esi+ebx]
	xor eax, eax
	mov ax, word ptr [esi]

	mov ebp, esp
	pop ebp
	ret 4
vm_read_opcode ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_read_dword_argument PROC
	push ebp
	mov ebp, esp

	; read vm ip
	mov ebp, esp
	mov eax, [ebp+arg0]
	mov ebx, [eax+vm_ip]

	; read dword argument
	mov esi, [eax+vm_code]
	lea esi, [esi+ebx]	
	mov eax, dword ptr [esi]

	mov ebp, esp
	pop ebp
	ret 4
vm_read_dword_argument ENDP

; *****************************
; arguments: vm_context, extracted opcode
; *****************************
vm_execute PROC
	push ebp
	mov ebp, esp		
	
	; find the handler
	mov ebx, [ebp+arg0]
	push [ebp+arg1]	
	push vm_instructions_size
	push start_vm_instructions
	call find_vm_handler

	; invoke the handler if found
	test eax, eax
	je end_execution

	push [ebp+arg0] ; all handlers take 1 argument which is the VM context
	call eax
	add esp, 4
	jmp end_execution

error:
	; TODO *****************************
	; set the error flag in context here

end_execution:
	mov ebp, esp
	pop ebp
	ret 8
vm_execute ENDP

; *****************************
; arguments: vm_context
; *****************************
vm_main PROC
	push ebp
	mov ebp, esp
		
vm_loop:		
	; read the opcode to execute		
	push [ebp+arg0]
	call vm_read_opcode
	push eax

	; increment the VM ip
	push 2
	push [ebp+arg0]
	call vm_increment_ip

	; execute the VM instruction
	push [ebp+arg0]
	call vm_execute
		
	; check the finish flag in the context
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_flags]
	test eax, 80000000h
	je vm_loop
	
	mov ebp, esp
	pop ebp
	ret 8
vm_main ENDP