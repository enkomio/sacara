comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 0E3h,0Fh,2h,0h,0h,0h           ; /* E30F02000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 69h,80h                        ; /*         6980 */ loc_00000006: VmAlloca
code_2 BYTE 0E3h,0Fh,12h,0h,0h,0h          ; /* E30F12000000 */ loc_00000008: VmPushImmediate 0x12
code_3 BYTE 7Bh,8Ah                        ; /*         7B8A */ loc_0000000E: VmJump
code_4 BYTE 1Dh,1h                         ; /*         1D01 */ loc_00000010: VmNop
code_5 BYTE 0ACh,8Fh,10h,0h,0h,0h          ; /* AC8F10000000 */ loc_00000012: VmPushImmediate 0x10
code_6 BYTE 93h,87h                        ; /*         9387 */ loc_00000018: VmRead
code_7 BYTE 0D2h,1h                        ; /*         D201 */ loc_0000001A: VmHalt
vm_code_bytes_size EQU $-code_0

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
	push offset code_0
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

