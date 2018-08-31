; local vars
local0 EQU -4h
local1 EQU -8h
local2 EQU -0Ch

; arguments
arg0 EQU 8h
arg1 EQU 0Ch
arg2 EQU 10h
arg3 EQU 14h

; VM stack frame
; each stack frame has the following header:
; <previous stack frame pointer>|<base stack frame>|<top stack frame>
vm_stack_size EQU 1024
vm_stack_vars_size EQU 1
vm_stack_previous_frame EQU 0h
vm_stack_base EQU 4h
vm_stack_top EQU 8h
vm_local_vars EQU 0Ch

; flags
VM_SIGN_FLAG EQU 010000000h

; VM context
vm_ip EQU 0h
vm_sp EQU 4h ; it is a list of stack pointer frames
vm_flags EQU 8h ; |halt flag|error flag|zero flag|sign flag|
vm_code EQU 0Ch
vm_code_size EQU 10h

; Windows constants
HEAP_ZERO_MEMORY EQU 00000008h

; Initial XOR opcode key
INIT_OPCODE_XOR_KEY EQU 05Bh
INIT_OPCODE_HEADER_XOR_KEY EQU 0B5h