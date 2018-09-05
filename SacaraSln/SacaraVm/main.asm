comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata
!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

.DATA
code_0000 BYTE 0A3h,88h,21h,0h,0h,0h                    ; /* A38821000000 */ loc_00000000: VmPushImmediate 0x21
code_0001 BYTE 4h,88h                                   ; /*         0488 */ loc_00000006: VmCall
code_0002 BYTE 76h,2h                                   ; /*         7602 */ loc_00000008: VmHalt
code_0003 BYTE 43h,69h,61h,6Fh,20h,61h,20h,74h,75h,74h,74h,69h,21h,21h,0h ; /* 4369616F2061207475747469212100 */ loc_0000000A: VmByte "Ciao a tutti!!",0
code_0004 BYTE 34h,12h,78h,56h                          ; /*     34127856 */ loc_00000019: VmWord 0x1234, 0x5678
code_0005 BYTE 78h,56h,34h,12h                          ; /*     78563412 */ loc_0000001D: VmDoubleWord 0x12345678
code_0006 BYTE 82h,88h,3h,0h,0h,0h                      ; /* 828803000000 */ loc_00000021: VmPushImmediate 0x3
code_0007 BYTE 0C8h,8Eh                                 ; /*         C88E */ loc_00000027: VmAlloca
code_0008 BYTE 8Ah,88h,0Ah,0h,0h,0h                     ; /* 8A880A000000 */ loc_00000029: VmPushImmediate 0xA
code_0009 BYTE 12h,84h,0h,0h                            ; /*     12840000 */ loc_0000002F: VmPop 0x0
code_0010 BYTE 0C1h,8Fh                                 ; /*         C18F */ loc_00000033: VmGetIp
code_0011 BYTE 78h,4h,1h,0h                             ; /*     78040100 */ loc_00000035: VmPop 0x1
code_0012 BYTE 85h,1h                                   ; /*         8501 */ loc_00000039: VmGetSp
code_0013 BYTE 1Eh,84h,2h,0h                            ; /*     1E840200 */ loc_0000003B: VmPop 0x2
code_0014 BYTE 49h,5h                                   ; /*         4905 */ loc_0000003F: VmRet
vm_code_bytes_size EQU $-code_0000
.CODE

include vm.asm

main PROC
	push ebp
	mov ebp, esp
		
	; allocate space on the stack for the VM context and initialize it	
	sub esp, 10h
	mov eax, vm_code_bytes_size
	push eax
	push offset code_0000
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

