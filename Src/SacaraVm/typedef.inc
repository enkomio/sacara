VmStackFrame STRUCT
	base DWORD 0h
	top DWORD 0h	
	previous DWORD 0h
	locals DWORD 0h
VmStackFrame ENDS

VmContext STRUCT
	ip DWORD 0h
	stack_frame DWORD 0h
	flags DWORD 0h ; |halt flag|<empty>|zero flag|carry flag|operands are encrypted|
	code DWORD 0h
	code_size DWORD 0h
	error_code DWORD 0h ; see error code constants in const.inc
	error_handler DWORD 0h ; an handler invoked when an error is encountered durign the emulation
VmContext ENDS

MODULEINFO STRUCT
  lpBaseOfDll DWORD ?
  SizeOfImage DWORD ?
  EntryPoint  DWORD ?
MODULEINFO ENDS

UNICODE_STRING STRUCT
	_Length WORD ?
	MaximumLength WORD ?
	Buffer DWORD ?
UNICODE_STRING ENDS