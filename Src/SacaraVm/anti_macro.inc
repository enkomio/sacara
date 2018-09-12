; *****************************
; arguments: VM context
; *****************************
check_debugger_via_trap_flag MACRO	
	assume fs:nothing
	pushad
	pushfd
	push check_debugger_seh_handler	
	push dword ptr fs:[0]
	mov dword ptr fs:[0], esp
	
	; set trap flag
	pushfd
	or dword ptr [esp], 0100h
	popfd

	; write garbage at the VM code
	mov eax, [ebp+arg0] ; get VM context
	inc dword ptr [eax+vm_ip]
	jmp check_debugger_finish

check_debugger_seh_handler:
	mov esp, [esp+8]	

check_debugger_finish:
	pop dword ptr fs:[0]
	add esp, 4

	popfd
	popad
	assume fs:error		
ENDM

; *****************************
; arguments: VM context
; *****************************
check_debugger_via_HeapAlloc MACRO
	pushad
	pushfd

	; GetProcessHeap
	push hash_kernel32_dll
	call find_module_base
	push hash_GetProcessHeap
	push eax
	call find_exported_func
	call eax
	
	; prepare arguments for RtlAllocateHeap
	push TYPE DWORD ; Size
	push 0h ; Flags
	push eax ; ProcessHeap

	; RtlAllocateHeap
	push hash_ntdll_dll
	call find_module_base
	push hash_RtlAllocateHeap
	push eax
	call find_exported_func
	call eax

	; do check
	mov eax, [eax]
	mov ebx, eax
	shr ebx, 10h
	xor bx, ax
	cmp bx, 04AA0h
	jne finish

	; write garbage at the VM code
	mov eax, [ebp+arg0] ; get VM context
	inc dword ptr [eax+vm_ip]

finish:

	popfd
	popad
ENDM