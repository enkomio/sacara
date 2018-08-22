comment !
This is the implementation of the Sacara VM.
2018 (C) Antonio 's4tan' Parata

An esaxmple of bytecode is:

/* 745300000002 */ 00000000: Alloca 00000002
/* 5DA20000000C */ 00000006: Jump 0000000C
/* E1D600000034 */ 0000000C: Push 00000034
/* E1D60000003E */ 00000012: Push 0000003E
/*         698B */ 00000018: Add 
/*     45780001 */ 0000001A: Pop 0001
/* 5DA20000002A */ 0000001E: Jump 0000002A
/* 1819000000FF */ 00000024: Byte 000000FF
/* 1D1B00000036 */ 0000002A: Call 00000036
/*     45780002 */ 00000030: Pop 0002
/*         8550 */ 00000034: Ret 
/*         176F */ 00000036: GetIp 
/*         8550 */ 00000038: Ret 

!

.386
.model flat,stdcall
.stack 4096
ExitProcess proto,dwExitCode:dword

include const.inc
include utility.inc

.code

main proc
	mov	eax,5				
	add	eax,6			
	call invoke_kernel32
	invoke ExitProcess,0
main endp
end main

