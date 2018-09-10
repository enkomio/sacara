header_VmHalt
vm_halt PROC
	push ebp
	mov ebp, esp
	mov ebx, [ebp+arg0] ; read context
	mov eax, [ebx+vm_flags]
	or eax, 80000000h ; set halt flag
	mov [ebx+vm_flags], eax
	mov esp, ebp
	pop ebp
	ret
vm_halt ENDP
header_end