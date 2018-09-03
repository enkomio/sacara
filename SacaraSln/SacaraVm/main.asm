comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 0CEh,0Ch,2h,0h,0h,0h           ; /* CE0C02000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 0A8h,83h                       ; /*         A883 */ loc_00000006: VmAlloca
code_2 BYTE 9Fh,8Ch,3Ch,0h,0h,0h           ; /* 9F8C3C000000 */ loc_00000008: VmPushImmediate 0x3C
code_3 BYTE 0CEh,0Ch,46h,0h,0h,0h          ; /* CE0C46000000 */ loc_0000000E: VmPushImmediate 0x46
code_4 BYTE 5Dh,8Ah                        ; /*         5D8A */ loc_00000014: VmCmp
code_5 BYTE 8Dh,8Ch,20h,0h,0h,0h           ; /* 8D8C20000000 */ loc_00000016: VmPushImmediate 0x20
code_6 BYTE 69h,83h                        ; /*         6983 */ loc_0000001C: VmJumpIfGreatEquals
code_7 BYTE 16h,87h                        ; /*         1687 */ loc_0000001E: VmHalt
code_8 BYTE 0B7h,8Ch,1Eh,0h,0h,0h          ; /* B78C1E000000 */ loc_00000020: VmPushImmediate 0x1E
code_9 BYTE 0BDh,8Ch,14h,0h,0h,0h          ; /* BD8C14000000 */ loc_00000026: VmPushImmediate 0x14
code_10 BYTE 65h,8Ah                        ; /*         658A */ loc_0000002C: VmCmp
code_11 BYTE 0CEh,0Ch,3Ah,0h,0h,0h          ; /* CE0C3A000000 */ loc_0000002E: VmPushImmediate 0x3A
code_12 BYTE 41h,83h                        ; /*         4183 */ loc_00000034: VmJumpIfGreatEquals
code_13 BYTE 87h,81h                        ; /*         8781 */ loc_00000036: VmNop
code_14 BYTE 0Ch,87h                        ; /*         0C87 */ loc_00000038: VmHalt
code_15 BYTE 0Ah,87h                        ; /*         0A87 */ loc_0000003A: VmHalt
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

