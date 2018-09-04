; This file is auto generated, don't modify it
marker1 EQU 0C0A49EF3h
marker2 EQU 0E07888CBh

; real opcodes: 0053Fh, 00DF3h, 00E4Dh
header_VmRet EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 0058Dh, 00D49h, 00EFBh>

; real opcodes: 00C54h, 00CC4h
header_VmNop EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00CE3h, 00C73h>

; real opcodes: 007B2h, 00DBBh
header_VmAdd EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00709h, 00D10h>

; real opcodes: 004A5h, 000FDh, 00271h, 00A4Fh, 00498h
header_VmPushImmediate EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 00415h, 0004Dh, 002C9h, 00AFFh, 00432h>

; real opcodes: 00FF9h, 00FD3h, 00B16h, 00CFCh
header_VmPushVariable EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 00F50h, 00F6Ah, 00BA7h, 00C4Dh>

; real opcodes: 00B94h, 006A2h
header_VmPop EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00B23h, 00619h>

; real opcodes: 0035Dh, 0003Fh
header_VmCall EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 003EAh, 0008Ch>

; real opcodes: 008DEh, 00A8Ah, 00674h, 00E28h, 00566h
header_VmJump EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 00870h, 00A44h, 006C6h, 00EA2h, 005D8h>

; real opcodes: 005FFh, 00ED3h, 00643h, 00D46h
header_VmAlloca EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 0054Eh, 00E6Ah, 006FAh, 00DF7h>

; real opcodes: 00FC6h, 003D8h, 0050Ch
header_VmHalt EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 00F76h, 00370h, 005BCh>

; real opcodes: 008D3h, 00E5Ch
header_VmCmp EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00868h, 00EEBh>

; real opcodes: 009F0h, 003A3h, 00D89h
header_VmStackWrite EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 00948h, 00319h, 00D3Fh>

; real opcodes: 00C76h, 0093Eh, 000EDh, 00367h, 00E07h
header_VmStackRead EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 00CC8h, 00990h, 0005Dh, 003D7h, 00EB7h>

; real opcodes: 009B5h, 0074Eh, 00C29h
header_VmJumpIfLess EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 00903h, 007FEh, 00C9Fh>

; real opcodes: 00C1Fh, 00DAFh, 001F2h, 00D32h
header_VmJumpIfLessEquals EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 00CAEh, 00D1Eh, 0014Bh, 00D8Bh>

; real opcodes: 00FBBh, 00F3Ch, 0062Bh, 00C7Bh, 00B86h
header_VmJumpIfGreat EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 00F13h, 00F8Eh, 006A3h, 00CD3h, 00B38h>

; real opcodes: 0017Fh, 00D83h
header_VmJumpIfGreatEquals EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 001CCh, 00D38h>

; real opcodes: 00748h, 007FAh, 0092Fh, 002BEh, 00844h
header_VmRead EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 00802h, 00754h, 0099Fh, 00210h, 008F6h>

; real opcodes: 00977h, 00CA5h, 00194h, 00609h
header_VmWrite EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 009C6h, 00C14h, 00125h, 006C0h>

; real opcodes: 00D34h, 00D44h
header_VmGetIp EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00D83h, 00DF3h>

; real opcodes: 00CD2h, 00906h, 0051Ch, 00EF3h
header_VmGetSp EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 00C6Bh, 009B7h, 005ADh, 00E4Ah>

; real opcodes: 00CB0h, 003B1h, 00632h, 00318h
header_VmNativeRead EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 00C09h, 00308h, 0068Bh, 003B1h>

; real opcodes: 0018Ch, 00EAAh, 00C52h, 003C3h
header_VmNativeWrite EQU <DWORD 0C0A49EF3h, 0E07888CBh, 4h, 0013Dh, 00E23h, 00CEBh, 0037Ah>

; real opcodes: 00201h, 0047Eh, 00B00h, 00BC5h, 000D4h
header_VmNativeCall EQU <DWORD 0C0A49EF3h, 0E07888CBh, 5h, 002B9h, 004D0h, 00BBAh, 00B75h, 00066h>

; real opcodes: 0060Bh, 003C6h
header_VmSub EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 006C0h, 00375h>

; real opcodes: 00F14h, 0058Bh
header_VmMul EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00FA3h, 00540h>

; real opcodes: 00F77h, 00C25h, 00BE6h
header_VmDiv EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 00FC5h, 00C93h, 00B56h>

; real opcodes: 00A1Ah, 00280h
header_VmNand EQU <DWORD 0C0A49EF3h, 0E07888CBh, 2h, 00AB1h, 00237h>

; real opcodes: 00963h, 00373h, 00F69h
header_VmShiftRight EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 009D9h, 003C9h, 00FDFh>

; real opcodes: 000D0h, 00C6Fh, 00114h
header_VmShiftLeft EQU <DWORD 0C0A49EF3h, 0E07888CBh, 3h, 00068h, 00CDDh, 001A4h>

header_end EQU <DWORD 0E07888CBh, 0C0A49EF3h>
