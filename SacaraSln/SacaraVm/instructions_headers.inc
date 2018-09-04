; This file is auto generated, don't modify it
marker1 EQU 0F18C399Bh
marker2 EQU 016FCF100h

; real opcodes: 00E76h, 00182h, 0007Bh, 0019Ch, 005BBh
header_VmRet EQU <DWORD 0F18C399Bh, 016FCF100h, 5h, 00EC8h, 0013Ch, 000D3h, 0012Eh, 00513h>

; real opcodes: 0090Ch, 009BDh
header_VmNop EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 009BBh, 0090Ah>

; real opcodes: 00593h, 00D60h, 00054h, 0010Ah
header_VmAdd EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 0052Ah, 00DD9h, 000E5h, 001C3h>

; real opcodes: 00CDEh, 005C5h, 00175h, 00D3Ch
header_VmPushImmediate EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 00C6Fh, 00574h, 001C4h, 00D8Dh>

; real opcodes: 000C6h, 00FC7h, 00DF4h, 00921h, 00B6Dh
header_VmPushVariable EQU <DWORD 0F18C399Bh, 016FCF100h, 5h, 00078h, 00F77h, 00D46h, 00999h, 00BDDh>

; real opcodes: 00276h, 00704h
header_VmPop EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 002C5h, 007B3h>

; real opcodes: 00676h, 00E39h, 0009Bh, 00BA5h, 002B0h
header_VmCall EQU <DWORD 0F18C399Bh, 016FCF100h, 5h, 006C8h, 00E91h, 00033h, 00B15h, 0020Ah>

; real opcodes: 007CBh, 008D7h, 00AFBh
header_VmJump EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 00781h, 00865h, 00A51h>

; real opcodes: 003DDh, 002DEh
header_VmAlloca EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 0036Ah, 0026Dh>

; real opcodes: 0060Ah, 00AEDh
header_VmHalt EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 006C1h, 00A5Ah>

; real opcodes: 00554h, 00422h, 000F4h
header_VmCmp EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 005E4h, 0049Ah, 00044h>

; real opcodes: 00A07h, 00C44h, 00F91h, 00C1Fh
header_VmStackWrite EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 00AB6h, 00CF5h, 00F28h, 00CAEh>

; real opcodes: 0066Fh, 00407h
header_VmStackRead EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 006DCh, 004B4h>

; real opcodes: 0086Bh, 0050Ch, 0015Fh, 0034Fh
header_VmJumpIfLess EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 008E2h, 005BDh, 001EEh, 003FEh>

; real opcodes: 00261h, 00344h, 00559h, 004F5h
header_VmJumpIfLessEquals EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 002D8h, 003F5h, 005F0h, 00444h>

; real opcodes: 00DA1h, 00F18h, 00752h
header_VmJumpIfGreat EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 00D17h, 00FB0h, 007EAh>

; real opcodes: 00AD3h, 005A3h
header_VmJumpIfGreatEquals EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 00A68h, 00518h>

; real opcodes: 00D6Fh, 005D3h, 009BAh
header_VmRead EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 00DDDh, 00569h, 00912h>

; real opcodes: 00565h, 00616h, 00C22h
header_VmWrite EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 005D3h, 006A6h, 00C9Ah>

; real opcodes: 00DC9h, 00CB7h, 00A3Ch, 00EB1h
header_VmGetIp EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 00D80h, 00C06h, 00A8Dh, 00E08h>

; real opcodes: 00832h, 00C4Ah
header_VmGetSp EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 00889h, 00D01h>

; real opcodes: 00748h, 00075h, 005F9h, 00A7Bh
header_VmNativeRead EQU <DWORD 0F18C399Bh, 016FCF100h, 4h, 00801h, 000C4h, 00550h, 00AD2h>

; real opcodes: 0045Ah, 00899h, 002C4h
header_VmNativeWrite EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 004F2h, 0082Fh, 00274h>

; real opcodes: 00ECEh, 003A5h, 0067Fh
header_VmNativeCall EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 00E7Eh, 00313h, 006CDh>

; real opcodes: 001EFh, 00600h, 0017Ah
header_VmSub EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 0015Dh, 006B8h, 001D2h>

; real opcodes: 002C0h
header_VmMul EQU <DWORD 0F18C399Bh, 016FCF100h, 1h, 00276h>

; real opcodes: 00A4Bh, 00D79h, 00303h
header_VmDiv EQU <DWORD 0F18C399Bh, 016FCF100h, 3h, 00B01h, 00DCFh, 003B9h>

; real opcodes: 00546h, 001C1h
header_VmNand EQU <DWORD 0F18C399Bh, 016FCF100h, 2h, 005F5h, 00176h>

header_end EQU <DWORD 016FCF100h, 0F18C399Bh>
