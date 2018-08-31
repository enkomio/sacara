; This file is auto generated, don't modify it
marker1 EQU 0C75C252Eh
marker2 EQU 0D0D4C208h

; real opcodes: 00471h, 00183h
header_VmRet EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 004C6h, 00138h>

; real opcodes: 001E4h, 00D4Bh, 002EEh
header_VmNop EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00154h, 00E01h, 0025Eh>

; real opcodes: 00CD3h, 0032Ah
header_VmAdd EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 00C68h, 003A1h>

; real opcodes: 00CCEh, 0024Ch
header_VmPushImmediate EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 00C7Dh, 002FBh>

; real opcodes: 00DE8h, 00E8Eh, 0028Bh, 00DAAh, 00BF9h
header_VmPushVariable EQU <DWORD 0C75C252Eh, 0D0D4C208h, 5h, 00D62h, 00E40h, 00243h, 00D24h, 00B51h>

; real opcodes: 003C6h, 0097Fh, 00DCCh, 0069Dh
header_VmPop EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00377h, 009CEh, 00D7Dh, 0062Ch>

; real opcodes: 006E8h, 00C03h, 00C52h, 001B7h
header_VmCall EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00661h, 00CBAh, 00CEBh, 00106h>

; real opcodes: 00AAFh, 0068Bh, 00515h, 009BAh
header_VmNativeCall EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00A1Eh, 00642h, 005A4h, 00913h>

; real opcodes: 0088Dh, 0059Ah
header_VmRead EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 0083Ah, 00531h>

; real opcodes: 00A6Dh, 008D7h, 0022Eh, 00294h
header_VmNativeRead EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00ADCh, 00866h, 0029Fh, 00225h>

; real opcodes: 003F1h, 002A0h, 006BEh
header_VmWrite EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00347h, 00218h, 0060Eh>

; real opcodes: 00B39h, 004E3h, 009FEh, 00245h
header_VmNativeWrite EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00B90h, 0045Ah, 0094Fh, 002F4h>

; real opcodes: 0028Ch, 00F99h, 008DCh, 00951h
header_VmGetIp EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 0023Dh, 00F30h, 0086Dh, 009E8h>

; real opcodes: 00AEFh, 00B07h, 00B05h
header_VmJump EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00A5Dh, 00BB5h, 00BB3h>

; real opcodes: 00699h, 00B04h
header_VmJumpIfLess EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 0062Eh, 00BB3h>

; real opcodes: 00884h, 00FD5h, 007B6h
header_VmJumpIfLessEquals EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00834h, 00F63h, 00706h>

; real opcodes: 00AA9h, 00559h, 008F2h
header_VmJumpIfGreat EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00A20h, 005F0h, 0084Bh>

; real opcodes: 0032Ch, 00036h, 0012Fh, 00F40h
header_VmJumpIfGreatEquals EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 0039Dh, 00087h, 0019Eh, 00FF9h>

; real opcodes: 003FBh, 00712h, 00F89h
header_VmAlloca EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00351h, 007AAh, 00F3Fh>

; real opcodes: 0076Dh
header_VmHalt EQU <DWORD 0C75C252Eh, 0D0D4C208h, 1h, 007DAh>

; real opcodes: 00A10h, 003FDh, 00579h, 000E7h
header_VmCmp EQU <DWORD 0C75C252Eh, 0D0D4C208h, 4h, 00AA9h, 0034Ch, 005D0h, 00056h>

; real opcodes: 001E8h, 00AA8h
header_VmGetSp EQU <DWORD 0C75C252Eh, 0D0D4C208h, 2h, 0015Fh, 00A1Fh>

; real opcodes: 00F65h, 00783h, 00B9Eh, 00799h, 00FE3h
header_VmStackWrite EQU <DWORD 0C75C252Eh, 0D0D4C208h, 5h, 00FD5h, 0073Bh, 00B30h, 00731h, 00F5Bh>

; real opcodes: 002D4h, 00F05h, 00EBEh
header_VmStackRead EQU <DWORD 0C75C252Eh, 0D0D4C208h, 3h, 00264h, 00FB3h, 00E0Eh>

header_end EQU <DWORD 0D0D4C208h, 0C75C252Eh>
