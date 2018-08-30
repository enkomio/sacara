comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 6Dh,0C6h,2h,0h,0h,0h           ; /* 6DC602000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 23h,2Dh                        ; /*         232D */ loc_00000006: VmAlloca
code_2 BYTE 30h,0D6h,12h,0h,0h,0h          ; /* 30D612000000 */ loc_00000008: VmJumpImmediate 0x12
code_3 BYTE 46h,34h                        ; /*         4634 */ loc_0000000E: VmNop
code_4 BYTE 46h,34h                        ; /*         4634 */ loc_00000010: VmNop
code_5 BYTE 46h,34h                        ; /*         4634 */ loc_00000012: VmNop
code_6 BYTE 6Dh,0C6h,1Eh,0h,0h,0h          ; /* 6DC61E000000 */ loc_00000014: VmPushImmediate 0x1E
code_7 BYTE 0E8h,22h                       ; /*         E822 */ loc_0000001A: VmCall
code_8 BYTE 6Fh,4Fh                        ; /*         6F4F */ loc_0000001C: VmHalt
code_9 BYTE 6Dh,0C6h,42h,0h,0h,0h          ; /* 6DC642000000 */ loc_0000001E: VmPushImmediate 0x42
code_10 BYTE 6Dh,0C6h,0F6h,0FFh,0FFh,0FFh   ; /* 6DC6F6FFFFFF */ loc_00000024: VmPushImmediate 0xFFFFFFF6
code_11 BYTE 2Fh,0DDh                       ; /*         2FDD */ loc_0000002A: VmCmp
code_12 BYTE 7h,0D7h                        ; /*         07D7 */ loc_0000002C: VmRet
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

