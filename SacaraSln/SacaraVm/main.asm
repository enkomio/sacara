comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

An esaxmple of bytecode is:

/* 5EF800000002 */ 00000000: Alloca 00000002
/* C1160000000C */ 00000006: Jump 0000000C
/* B43A00000034 */ 0000000C: Push 00000034
/* B43A0000003E */ 00000012: Push 0000003E
/*         4A4F */ 00000018: Add
/*     14170001 */ 0000001A: Pop 0001
/* C1160000002A */ 0000001E: Jump 0000002A
/* EF2C000000FF */ 00000024: Byte 000000FF
/* 660400000038 */ 0000002A: Call 00000038
/*     14170002 */ 00000030: Pop 0002
/*         FCBE */ 00000034: Halt
/*         6B56 */ 00000036: Ret
/*         9447 */ 00000038: GetIp
/*         6B56 */ 0000003A: Ret

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.data

vm_code_bytes BYTE 5Eh,0F8h,0h,0h,0h,2h,0C1h,16h,0h,0h,0h,0Ch,0B4h,3Ah,0h,0h
		  BYTE 0h,34h,0B4h,3Ah,0h,0h,0h,3Eh,4Ah,4Fh,14h,17h,0h,1h,0C1h,16h
		  BYTE 0h,0h,0h,2Ah,0EFh,2Ch,0h,0h,0h,0FFh,66h,4h,0h,0h,0h,38h,14h
		  BYTE 17h,0h,2h,0FCh,0BEh,6Bh,56h,94h,47h,6Bh,56h
vm_code_size EQU $-vm_code_bytes

.code

include const.inc
include utility.inc
include vm_handlers.inc

; compute size of the code related to the VM
start_vm_code:
include vm.inc
vm_code_lib_size DWORD $ - start_vm_code

main PROC
	push ebp
	mov ebp, esp
	
	; test find marker
	push 0918h
	push vm_code_lib_size
	push start_vm_code
	call find_marker

	test eax, eax
	je error
	call eax

error:

	mov eax, vm_code_lib_size

	; allocate space on the stack for the VM context and initialize it	
	sub esp, 0Ch
	push ebp
	call vm_init
	
	; run VM
	push offset vm_code_bytes
	push ebp
	call vm_main

	;call test_label
test_label:
	;pop eax
	;mov	eax,$
	;add	eax, vm.vm_ip			
	;call invoke_kernel32
	invoke ExitProcess,0

	mov ebp, esp
	pop ebp
	ret
main ENDP
END main

