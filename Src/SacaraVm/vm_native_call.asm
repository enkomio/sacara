header_VmNativeCall
vm_native_call PROC
	push ebp
	mov ebp, esp
	sub esp, 08h
	
	; save native address
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local0], eax

	; save number of parameters
	push [ebp+arg0]
	call_vm_stack_pop_enc
	mov [ebp+local1], eax
		
	; push the managed value into the native stack
	mov ecx, [ebp+local1]
	test ecx, ecx
	jz invoke_native_code

	; compute new stack pointer
	lea esi, [ecx * type dword]
	sub esp, esi
	mov edi, esp

arguments_push:
	; get argument to copy
	push edi
	push ecx
	push [ebp+arg0]
	call_vm_stack_pop_enc
	pop ecx
	pop edi

	; copy argument to native stack
	mov dword ptr [edi], eax
	add edi, 4
	loop arguments_push
	
	; invoke native code
invoke_native_code:
	call dword ptr [ebp+local0]
	
save_return_value:
	push eax ; push return value
	push [ebp+arg0]
	call_vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_native_call ENDP
header_marker