comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 37h,0BDh,2h,0h,0h,0h           ; /* 37BD02000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 0D7h,13h                       ; /*         D713 */ loc_00000006: VmAlloca
code_2 BYTE 37h,0BDh,14h,0h,0h,0h          ; /* 37BD14000000 */ loc_00000008: VmPushImmediate 0x14
code_3 BYTE 0D7h,41h                       ; /*         D741 */ loc_0000000E: VmJump
code_4 BYTE 4h,7Eh                         ; /*         047E */ loc_00000010: VmNop
code_5 BYTE 4h,7Eh                         ; /*         047E */ loc_00000012: VmNop
code_6 BYTE 4h,7Eh                         ; /*         047E */ loc_00000014: VmNop
code_7 BYTE 37h,0BDh,20h,0h,0h,0h          ; /* 37BD20000000 */ loc_00000016: VmPushImmediate 0x20
code_8 BYTE 0E4h,50h                       ; /*         E450 */ loc_0000001C: VmCall
code_9 BYTE 0FAh,0CEh                      ; /*         FACE */ loc_0000001E: VmHalt
code_10 BYTE 37h,0BDh,42h,0h,0h,0h          ; /* 37BD42000000 */ loc_00000020: VmPushImmediate 0x42
code_11 BYTE 37h,0BDh,0F6h,0FFh,0FFh,0FFh   ; /* 37BDF6FFFFFF */ loc_00000026: VmPushImmediate 0xFFFFFFF6
code_12 BYTE 92h,0B8h                       ; /*         92B8 */ loc_0000002C: VmCmp
code_13 BYTE 8Eh,0DEh                       ; /*         8EDE */ loc_0000002E: VmRet
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

