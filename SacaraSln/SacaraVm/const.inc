local0 EQU -4h
local1 EQU -8h
local2 EQU -0Ch

arg0 EQU 8h
arg1 EQU 0Ch
arg2 EQU 10h
arg3 EQU 14h

; the size of the stack includes also the metadata
; each stack frame has the following header:
; <previous stack frame pointer>|<base stack frame>|<top stack frame>
vm_stack_size EQU 1024
vm_stack_vars_size EQU 1024
vm_stack_previous_frame EQU 0h
vm_stack_base EQU 4h
vm_stack_top EQU 8h
vm_local_vars EQU 0Ch

; VM context
vm_ip EQU 0h
vm_sp EQU 4h ; it is a list of stack pointer frames
vm_flags EQU 8h ; |halt bit|...|
vm_code EQU 0Ch
vm_code_size EQU 10h

; Windows constants
HEAP_ZERO_MEMORY EQU 00000008h