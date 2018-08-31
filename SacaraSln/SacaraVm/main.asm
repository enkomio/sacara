comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 97h,8Ch,1h,0h,0h,0h            ; /* 978C01000000 */ loc_00000000: VmPushImmediate 0x1
code_1 BYTE 0A8h,83h                       ; /*         A883 */ loc_00000006: VmAlloca
code_2 BYTE 9Fh,8Ch,16h,0h,0h,0h           ; /* 9F8C16000000 */ loc_00000008: VmPushImmediate 0x16
code_3 BYTE 0C6h,3h,0h,0h                  ; /*     C6030000 */ loc_0000000E: VmPop 0x0
code_4 BYTE 0CEh,0Ch,2Ch,0h,0h,0h          ; /* CE0C3C000000 */ loc_00000012: VmPushImmediate 0x3C
code_5 BYTE 8Fh,8Ch,32h,0h,0h,0h           ; /* 8F8C32000000 */ loc_00000018: VmPushImmediate 0x32
code_6 BYTE 6Bh,8Ah                        ; /*         6B8A */ loc_0000001E: VmCmp
code_7 BYTE 0B7h,8Ch,2Ah,0h,0h,0h          ; /* B78C2A000000 */ loc_00000020: VmPushImmediate 0x2A
code_8 BYTE 0EAh,86h                       ; /*         EA86 */ loc_00000026: VmJumpIfLess
code_9 BYTE 1Ch,87h                        ; /*         1C87 */ loc_00000028: VmHalt
code_10 BYTE 0B9h,8Ch,2Ch,0h,0h,0h          ; /* B98C2C000000 */ loc_0000002A: VmPushImmediate 0x2C
code_11 BYTE 4h,87h                         ; /*         0487 */ loc_00000030: VmHalt
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

