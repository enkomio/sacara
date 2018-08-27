comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

An esaxmple of bytecode is:

/* 2E9500000002 */ 00000000: Alloca 00000002
/* 72FE0000000C */ 00000006: Jump 0000000C
/* 15D800000034 */ 0000000C: Push 00000034
/* 15D80000003E */ 00000012: Push 0000003E
/*         02C7 */ 00000018: Add
/*     E4720001 */ 0000001A: Pop 0001
/* 72FE0000002A */ 0000001E: Jump 0000002A
/* 1215000000FF */ 00000024: Byte 000000FF
/* F2B100000038 */ 0000002A: Call 00000038
/*     E4720002 */ 00000030: Pop 0002
/*         69BB */ 00000034: Halt
/*         CE28 */ 00000036: Ret
/*         9392 */ 00000038: GetIp
/*         CE28 */ 0000003A: Ret
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.data

vm_code_bytes BYTE 18h,09h ; nop
				BYTE 0BCh,02Bh,02h,00h,00h,00h ; alloca 2
				BYTE 93h,38h ; halt
vm_code_bytes_size EQU $-vm_code_bytes

.code

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

	push hash_kernel32_dll
	call find_module_base
	
	push hash_GetProcessHeap
	push eax
	call find_exported_func
	
	call eax ; call GetProcessHeap
	
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

	; cleanup stack from vm_context structure
	add esp, 10h

	; exit
	invoke ExitProcess,0

	mov ebp, esp
	pop ebp
	ret
main ENDP
END main

