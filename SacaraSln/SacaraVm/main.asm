comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 97h,8Ch,2h,0h,0h,0h            ; /* 978C02000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 0FBh,3h                        ; /*         FB03 */ loc_00000006: VmAlloca
code_2 BYTE 9Fh,8Ch,14h,0h,0h,0h           ; /* 9F8C14000000 */ loc_00000008: VmPushImmediate 0x14
code_3 BYTE 0A4h,8Ah                       ; /*         A48A */ loc_0000000E: VmJump
code_4 BYTE 0ADh,81h                       ; /*         AD81 */ loc_00000010: VmNop
code_5 BYTE 0ABh,81h                       ; /*         AB81 */ loc_00000012: VmNop
code_6 BYTE 0A9h,81h                       ; /*         A981 */ loc_00000014: VmNop
code_7 BYTE 8Dh,8Ch,20h,0h,0h,0h           ; /* 8D8C20000000 */ loc_00000016: VmPushImmediate 0x20
code_8 BYTE 0E8h,6h                        ; /*         E806 */ loc_0000001C: VmCall
code_9 BYTE 16h,87h                        ; /*         1687 */ loc_0000001E: VmHalt
code_10 BYTE 0CEh,0Ch,42h,0h,0h,0h          ; /* CE0C42000000 */ loc_00000020: VmPushImmediate 0x42
code_11 BYTE 0CEh,0Ch,0F6h,0FFh,0FFh,0FFh   ; /* CE0CF6FFFFFF */ loc_00000026: VmPushImmediate 0xFFFFFFF6
code_12 BYTE 65h,8Ah                        ; /*         658A */ loc_0000002C: VmCmp
code_13 BYTE 1Ah,84h                        ; /*         1A84 */ loc_0000002E: VmRet
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

