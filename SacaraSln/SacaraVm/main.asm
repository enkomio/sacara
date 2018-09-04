comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0 BYTE 0BAh,8Fh,2h,0h,0h,0h           ; /* BA8F02000000 */ loc_00000000: VmPushImmediate 0x2
code_1 BYTE 69h,80h                        ; /*         6980 */ loc_00000006: VmAlloca
code_2 BYTE 0B2h,8Fh,2Ch,0h,0h,0h          ; /* B28F2C000000 */ loc_00000008: VmPushImmediate 0x2C
code_3 BYTE 0A8h,8Fh,16h,0h,0h,0h          ; /* A88F16000000 */ loc_0000000E: VmPushImmediate 0x16
code_4 BYTE 7Dh,8Ah                        ; /*         7D8A */ loc_00000014: VmJump
code_5 BYTE 92h,8Bh                        ; /*         928B */ loc_00000016: VmGetIp
code_6 BYTE 4Bh,8Ah,0h,0h                  ; /*     4B8A0000 */ loc_00000018: VmPop 0x0
code_7 BYTE 8Eh,85h                        ; /*         8E85 */ loc_0000001C: VmGetSp
code_8 BYTE 71h,8Ah,1h,0h                  ; /*     718A0100 */ loc_0000001E: VmPop 0x1
code_9 BYTE 0ADh,81h                       ; /*         AD81 */ loc_00000022: VmHalt
vm_code_bytes_size EQU $-code_0

.CODE

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

