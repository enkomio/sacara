VmStackFrame STRUCT
	base DWORD 0h
	top DWORD 0h	
	previous DWORD 0h
	locals DWORD 0h
VmStackFrame ENDS

VmContext STRUCT
	ip DWORD 0h
	stack_frame DWORD 0h
	flags DWORD 0h ; |halt flag|error flag|zero flag|carry flag|operands are encrypted|
	code DWORD 0h
	code_size DWORD 0h
VmContext ENDS