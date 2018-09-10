header_VmRet
vm_ret PROC
	push ebp
	mov ebp, esp
	sub esp, 0Ch

	; check if the stack is empty
	push [ebp+arg0]
	call vm_is_stack_empty
	cmp eax, 1
	jz remove_stack_frame

	; save to local var the value to return to the previous function
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov [ebp+local0], eax
	mov dword ptr [ebp+local2], 1h

remove_stack_frame:
	; save previous stack frame to local var
	mov eax, [ebp+arg0]
	mov eax, [eax+vm_sp]
	mov eax, [eax+vm_stack_previous_frame]
	mov [ebp+local1], eax

	; free current stack pointer
	push [ebp+arg0]
	call vm_free

	; set the previous stack pointer
	mov ebx, [ebp+local1]
	mov eax, [ebp+arg0]
	mov [eax+vm_sp], ebx

	; reset the saved vm IP if the current stack is not empty
	push [ebp+arg0]
	call vm_is_stack_empty
	cmp eax, 1
	jz push_return_value
		
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov ebx, [ebp+arg0]
	mov [ebx+vm_ip], eax

	; push the return value if necessary
push_return_value:
	cmp dword ptr [ebp+local2], 1h
	jne finish

	push [ebp+local0]
	push [ebp+arg0]
	call vm_stack_push_enc

finish:	
	add esp, 0Ch
	mov ebp, esp
	pop ebp
	ret
vm_ret ENDP
header_end