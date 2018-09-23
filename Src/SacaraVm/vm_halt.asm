header_VmHalt
vm_halt PROC
	push ebp
	mov ebp, esp
	mov ebx, [ebp+arg0] ; read context
	mov eax, (VmContext PTR [ebx]).flags
	or eax, 80000000h ; set halt flag
	mov (VmContext PTR [ebx]).flags, eax
	mov esp, ebp
	pop ebp
	ret
vm_halt ENDP
header_marker