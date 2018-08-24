local0 EQU -4h
local1 EQU -8h
local2 EQU -0Ch

arg0 EQU 8h
arg1 EQU 0Ch
arg2 EQU 10h
arg3 EQU 14h

; VM context
vm_ip EQU 0h
vm_sp EQU 4h ; this should be activation frames linked list
vm_flags EQU 8h ; |halt bit|error bit|
vm_code EQU 0Ch
vm_code_size EQU 10h