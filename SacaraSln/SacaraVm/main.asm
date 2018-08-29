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

.DATA

vm_code_bytes BYTE 0F9h,93h,02h,0h,0h,0h		; push 2. Stack = 2
				BYTE 1Dh,52h,05h,0h,0h,0h		; alloca 5, create space for 1 variable
				BYTE 4Bh,87h,01h,0h				; pop <1>, set the variable 1 to 2.. Stack = <>, vars= 0|2
				BYTE 0F9h,93h,01h,0h,0h,0h		; push_immediate 1. Stack = 1
				BYTE 0FDh,0F3h,01h,0h			; push <1>, push the value of the variable 1 into the stack. Stack = 1|2
				BYTE 0F9h,93h,04h,0h,0h,0h		; push_immediate 4. Stack = 1|2|4
				BYTE 0F9h,93h,05h,0h,0h,0h		; push_immediate 5. Stack = 1|2|4|5
				BYTE 0F9h,93h,02h,0h,0h,0h		; push_immediate 2. Stack = 1|2|4|5|2
				BYTE 07Bh,081h					; sread. read the value of the stack at offset 2 and put it on top. Stack = 1|2|4|5|4
				BYTE 0F9h,93h,03h,0h,0h,0h		; push_immediate 3. Stack = 1|2|4|5|4|3
				BYTE 94h,0DBh					; swrite; Stack = 1|2|4|4
				BYTE 93h,38h					; halt
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

