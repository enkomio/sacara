comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 6Dh,0C6h,1h,0h,0h,0h           ; /* 6DC601000000 */ loc_00000000: VmPushImmediate 0x1
code_1 BYTE 23h,2Dh                        ; /*         232D */ loc_00000006: VmAlloca
code_2 BYTE 6Dh,0C6h,12h,0h,0h,0h          ; /* 6DC612000000 */ loc_00000008: VmPushImmediate 0x12
code_3 BYTE 0E8h,22h                       ; /*         E822 */ loc_0000000E: VmCall
code_4 BYTE 6Fh,4Fh                        ; /*         6F4F */ loc_00000010: VmHalt
code_5 BYTE 6Dh,0C6h,42h,0h,0h,0h          ; /* 6DC642000000 */ loc_00000012: VmPushImmediate 0x42
;code_6 BYTE 6Dh,0C6h,0F6h,0FFh,0FFh,0FFh   ; /* 6DC6F6FFFFFF */ loc_00000018: VmPushImmediate 0xFFFFFFF6
code_6 BYTE 6Dh,0C6h,30h,0h,0h,0h
code_7 BYTE 2Fh,0DDh                       ; /*         2FDD */ loc_0000001E: VmCmp
code_8 BYTE 7h,0D7h                        ; /*         07D7 */ loc_00000020: VmRet
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

