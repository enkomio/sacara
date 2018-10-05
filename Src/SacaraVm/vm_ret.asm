header_VmRet
vm_ret PROC
	push ebp
	mov ebp, esp
	sub esp, 0Ch

	; check if the stack is empty
	push [ebp+arg0]
	call_vm_is_stack_empty
	cmp eax, 1
	jz remove_stack_frame

	; save to local var the value to return to the previous function
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local0], eax
	mov dword ptr [ebp+local2], 1h

remove_stack_frame:
	; save previous stack frame to local var
	mov eax, [ebp+arg0]
	mov eax, (VmContext PTR [eax]).stack_frame
	mov eax, (VmStackFrame PTR [eax]).previous
	mov [ebp+local1], eax

	; free current stack pointer
	push [ebp+arg0]
	call_vm_free_frame

	; set the previous stack pointer
	mov ebx, [ebp+local1]
	mov eax, [ebp+arg0]
	mov (VmContext PTR [eax]).stack_frame, ebx

	; reset the saved vm IP if the current stack is not empty
	push [ebp+arg0]
	call_vm_is_stack_empty
	cmp eax, 1
	jz push_return_value
		
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov ebx, [ebp+arg0]
	mov (VmContext PTR [ebx]).ip, eax

	; push the return value if necessary
push_return_value:
	cmp dword ptr [ebp+local2], 1h
	jne finish

	push [ebp+local0]
	push [ebp+arg0]
	call_vm_stack_push_enc

finish:	
	mov esp, ebp
	pop ebp
	ret
vm_ret ENDP
header_marker