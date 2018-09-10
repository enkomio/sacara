header_VmNativeCall
vm_native_call PROC
	push ebp
	mov ebp, esp
	sub esp, 0Ch
	
	; save native address
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov [ebp+local0], eax

	; save number of parameters
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov [ebp+local1], eax
	
	; get flag to see if we have to clean the stack
	push [ebp+arg0]
	call vm_stack_pop_enc
	mov [ebp+local2], eax

	; push the managed value into the native stack
	mov ecx, [ebp+local1]
	test ecx, ecx
	jz invoke_native_code

arguments_push:
	push ecx; save loop counter
	push [ebp+arg0]
	call vm_stack_pop_enc
	pop ecx ; restore counter
	push eax ; push argument into the native stack
	loop arguments_push	
	
	; invoke native code
invoke_native_code:
	call dword ptr [ebp+local0]

	; check if we have to clean the stack
	cmp dword ptr [ebp+local2], 0h
	jz save_return_value
	mov ecx, [ebp+local1]
	lea ecx, [ecx*TYPE DWORD]
	add esp, ecx

save_return_value:
	push eax ; push return value
	push [ebp+arg0]
	call vm_stack_push_enc

	mov esp, ebp
	pop ebp
	ret
vm_native_call ENDP
header_end