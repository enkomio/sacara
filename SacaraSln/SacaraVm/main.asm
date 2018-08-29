comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

An esaxmple of bytecode is:

/* 722202000000 */ 00000000: Push 00000002
/*         AB53 */ 00000006: Alloca
/*     B1E20100 */ 00000008: Push 0001
/* 722220000000 */ 0000000C: Push 00000020
/*         023B */ 00000012: Call
/*     429A0200 */ 00000014: Pop 0002
/* 722214000000 */ 00000018: Push 00000014
/*         30E3 */ 0000001E: Ret
/* 722242000000 */ 00000020: Push 00000042
/*         30E3 */ 00000026: Ret

MASM: ,0B1h,0E2h,1h,0h,72h,22h,20h,0h,0h,0h,2h,3Bh,42h,9Ah,2h,0h,72h,22h,14h,0h,0h,0h,30h,0E3h,72h,22h,42h,0h,0h,0h,30h,0E3h

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA

vm_code_bytes BYTE 72h,22h,2h,0h,0h,0h ; push_immediate 2
			BYTE 0ABh,53h ; alloca
			BYTE 0BCh,01h ; halt
vm_code_bytes_size EQU $-vm_code_bytes

.CODE

include const.inc
include strings.inc
include utility.asm
include vm_instructions_headers.inc

; compute size of the code related to the VM. 
; These offset are used by the find_vm_handler routine
start_vm_instructions:
include vm_instructions.inc
vm_instructions_size DWORD $ - start_vm_instructions

include vm.asm

main PROC
	push ebp
	mov ebp, esp
	
	; allocate space on the stack for the VM context and initialize it	
	sub esp, 10h
	mov eax, vm_code_bytes_size
	push eax
	push offset vm_code_bytes
	push ebp
	call vm_init
	
	; run VM
	push ebp
	call vm_main

	; free vm
	push ebp
	call vm_free

	; cleanup stack from vm_context structure
	add esp, 10h

	; exit
	invoke ExitProcess,0

	mov ebp, esp
	pop ebp
	ret
main ENDP
END main

