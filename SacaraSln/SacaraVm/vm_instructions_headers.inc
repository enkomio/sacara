; This file is auto generated, don't modify it
marker1 EQU 01EA8890Dh
marker2 EQU 0DF54AE9Dh

; real opcodes: 00554h, 0086Fh
header_VmRet EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 005E3h, 008DCh>

; real opcodes: 0011Dh, 00826h, 003DAh
header_VmNop EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 001ABh, 00896h, 00372h>

; real opcodes: 007A2h, 00C64h, 00C9Fh
header_VmAdd EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 0071Ah, 00CD4h, 00C2Dh>

; real opcodes: 00FE3h, 00216h
header_VmPushImmediate EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00F58h, 002A5h>

; real opcodes: 003B3h, 00A77h
header_VmPushVariable EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00308h, 00AC4h>

; real opcodes: 00A0Ah, 003BCh, 002C0h
header_VmPop EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 00AC2h, 0030Ch, 00278h>

; real opcodes: 00298h, 0071Fh, 006A1h, 00828h
header_VmCall EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 4h, 00231h, 007AEh, 00618h, 008A1h>

; real opcodes: 00A30h, 005BBh, 004F5h, 00C2Ch
header_VmJump EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 4h, 00A89h, 00512h, 00444h, 00C9Dh>

; real opcodes: 0003Ah, 00FCEh, 00E1Fh
header_VmAlloca EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 00092h, 00F7Eh, 00EADh>

; real opcodes: 001D2h, 00999h
header_VmHalt EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00169h, 0092Eh>

; real opcodes: 00FB0h, 00ACDh
header_VmCmp EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00F07h, 00A7Ah>

; real opcodes: 009B8h, 00C04h, 00784h, 00308h
header_VmStackWrite EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 4h, 00911h, 00CB5h, 00735h, 003C1h>

; real opcodes: 00079h, 00DF3h
header_VmStackRead EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 000CEh, 00D48h>

; real opcodes: 00D13h, 00BC3h
header_VmJumpIfLess EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00DA8h, 00B78h>

; real opcodes: 006EDh, 00636h, 002C5h, 0015Ah, 0099Dh
header_VmJumpIfLessEquals EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 5h, 0065Dh, 00688h, 00275h, 001F4h, 0092Dh>

; real opcodes: 00AA7h, 009A7h, 00F6Ch, 002B7h, 00036h
header_VmJumpIfGreat EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 5h, 00A17h, 00917h, 00FDEh, 00207h, 00088h>

; real opcodes: 00DBAh, 00012h, 003F5h, 00D19h, 001DFh
header_VmJumpIfGreatEquals EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 5h, 00D14h, 000ACh, 00345h, 00DB1h, 0016Fh>

; real opcodes: 007D2h, 00A63h, 008EDh
header_VmRead EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 0076Ah, 00AD9h, 0085Bh>

; real opcodes: 00D28h, 0069Eh
header_VmWrite EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 00D9Fh, 0062Dh>

; real opcodes: 00BD1h, 00529h, 00AC7h, 009C1h
header_VmGetIp EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 4h, 00B68h, 005A0h, 00A76h, 00978h>

; real opcodes: 005CBh, 00F17h, 00945h, 00D90h, 008DEh
header_VmGetSp EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 5h, 00583h, 00FA7h, 009F5h, 00D2Ah, 00870h>

; real opcodes: 00F98h, 00076h, 00006h
header_VmNativeRead EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 3h, 00F30h, 000C6h, 000B6h>

; real opcodes: 00507h, 0012Fh
header_VmNativeWrite EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 2h, 005B4h, 0019Ch>

; real opcodes: 0070Bh, 00BCCh, 00FE5h, 00D1Dh
header_VmNativeCall EQU <DWORD 01EA8890Dh, 0DF54AE9Dh, 4h, 007C2h, 00B7Dh, 00F54h, 00DACh>

header_end EQU <DWORD 0DF54AE9Dh, 01EA8890Dh>
