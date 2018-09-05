; This file is auto generated, don't modify it
marker1 EQU 08F80EFAEh
marker2 EQU 0B02CEF1Eh

; real opcodes: 00549h, 00924h, 0062Bh
header_VmRet EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 005FFh, 00994h, 006A1h>

; real opcodes: 00A38h, 00FDAh, 00BEFh, 00400h, 0022Ch
header_VmNop EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 00A92h, 00F74h, 00B5Fh, 004BAh, 0029Eh>

; real opcodes: 00E72h, 00ED3h, 00A8Fh, 009D4h
header_VmAdd EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00ECBh, 00E6Ah, 00A3Eh, 00965h>

; real opcodes: 008FAh, 0005Bh, 00081h
header_VmPushImmediate EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 00852h, 000F1h, 00037h>

; real opcodes: 00657h, 00664h, 00329h, 00128h, 0047Fh
header_VmPushVariable EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 006E7h, 006D6h, 003A1h, 001A2h, 004CFh>

; real opcodes: 00478h, 00831h, 0030Eh
header_VmPop EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 004D0h, 00887h, 003BEh>

; real opcodes: 00857h, 009C1h, 000D8h, 0002Ch, 00FECh
header_VmCall EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 008E7h, 00979h, 00072h, 0009Eh, 00F5Eh>

; real opcodes: 0078Ah, 009C3h
header_VmJump EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00741h, 00978h>

; real opcodes: 00EBAh, 009D3h
header_VmAlloca EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00E11h, 00968h>

; real opcodes: 00276h, 00B6Ch, 008AEh, 001A2h
header_VmHalt EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 002C7h, 00BDDh, 0081Fh, 0011Bh>

; real opcodes: 001A6h, 00614h, 00D51h, 00FE2h
header_VmCmp EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00117h, 006A5h, 00DE8h, 00F5Bh>

; real opcodes: 00A1Ch, 00138h, 00DF8h, 00B6Bh
header_VmStackWrite EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00AADh, 00191h, 00D51h, 00BE2h>

; real opcodes: 007F9h, 001AAh, 001BAh, 005BDh
header_VmStackRead EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00750h, 00123h, 00113h, 0050Ch>

; real opcodes: 008DEh, 004B6h
header_VmJumpIfLess EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 0086Dh, 00405h>

; real opcodes: 002B3h, 00D97h, 00EA4h
header_VmJumpIfLessEquals EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 00209h, 00D25h, 00E14h>

; real opcodes: 00639h, 00A7Ch, 00F34h
header_VmJumpIfGreat EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 0068Fh, 00ACCh, 00F84h>

; real opcodes: 001C5h, 00F74h
header_VmJumpIfGreatEquals EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00172h, 00FC3h>

; real opcodes: 003F8h, 007C7h, 00877h
header_VmRead EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 00350h, 00775h, 008C5h>

; real opcodes: 006AFh, 002DCh, 00226h, 005BAh, 00F70h
header_VmWrite EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 0061Fh, 0026Eh, 00298h, 00514h, 00FCAh>

; real opcodes: 00FAFh, 00FAAh, 00232h
header_VmGetIp EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 00F1Dh, 00F22h, 0028Ah>

; real opcodes: 00185h, 00F8Eh, 00817h
header_VmGetSp EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 00133h, 00F3Eh, 008A5h>

; real opcodes: 00907h, 0022Ah, 009F0h, 00DCAh, 0018Bh
header_VmNativeRead EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 009B7h, 002A4h, 0094Ah, 00D84h, 00143h>

; real opcodes: 00408h, 00F96h
header_VmNativeWrite EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 004BFh, 00F25h>

; real opcodes: 00FD9h, 002DFh
header_VmNativeCall EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00F6Eh, 0026Ch>

; real opcodes: 0010Ah, 00860h, 001C1h, 00A72h, 0077Dh
header_VmByte EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 001C4h, 008DAh, 00179h, 00ACCh, 007CDh>

; real opcodes: 00F8Ch, 00ECDh, 00B09h, 002EBh, 0058Ah
header_VmWord EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 5h, 00F3Eh, 00E7Dh, 00BC1h, 00263h, 00544h>

; real opcodes: 00543h, 004F1h
header_VmDoubleWord EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 005F8h, 00446h>

; real opcodes: 00DF7h, 00C7Ch, 00620h, 004BEh
header_VmSub EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00D46h, 00CCDh, 00699h, 0040Fh>

; real opcodes: 00727h, 00629h, 008C8h, 00D48h
header_VmMul EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00796h, 006A0h, 00881h, 00E01h>

; real opcodes: 0062Ch, 0027Ch, 00EDCh
header_VmDiv EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 3h, 0069Ch, 002CCh, 00E6Ch>

; real opcodes: 00BD6h, 0024Bh
header_VmNand EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00B65h, 00300h>

; real opcodes: 00EC2h, 003DEh, 0074Ch, 00DE6h
header_VmShiftRight EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 4h, 00E7Bh, 0036Fh, 007FDh, 00D57h>

; real opcodes: 00D17h, 0072Fh
header_VmShiftLeft EQU <DWORD 08F80EFAEh, 0B02CEF1Eh, 2h, 00DA4h, 0079Ch>

header_end EQU <DWORD 0B02CEF1Eh, 08F80EFAEh>
