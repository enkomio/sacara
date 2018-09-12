; *****************************
; arguments: VM context as first parameter
; *****************************
check_debugger MACRO	
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