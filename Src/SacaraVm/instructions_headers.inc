; This file is auto generated, don't modify it
marker1 EQU 0A998A302h
marker2 EQU 0AE307CA9h

; real opcodes: 0099Ah, 00117h
header_VmRet EQU <DWORD marker1, marker2, 2h, 00931h, 001A4h>

; real opcodes: 00D70h, 00DFEh, 00FD8h, 002E7h, 0032Eh
header_VmNop EQU <DWORD marker1, marker2, 5h, 00DCAh, 00D50h, 00F72h, 00257h, 003A0h>

; real opcodes: 00B0Eh, 00283h, 007FDh, 0015Bh
header_VmAdd EQU <DWORD marker1, marker2, 4h, 00BBFh, 0023Ah, 0074Ch, 001F2h>

; real opcodes: 00ECFh, 0037Bh, 00506h, 00E07h, 0068Bh
header_VmPushImmediate EQU <DWORD marker1, marker2, 5h, 00E7Fh, 003D3h, 005B8h, 00EB7h, 00643h>

; real opcodes: 00DFDh, 0074Ah, 007BCh, 00675h, 00EDAh
header_VmPushVariable EQU <DWORD marker1, marker2, 5h, 00D4Dh, 00804h, 0070Eh, 006C5h, 00E74h>

; real opcodes: 00B02h, 00CBEh, 0054Dh, 0045Fh
header_VmPop EQU <DWORD marker1, marker2, 4h, 00BBBh, 00C0Fh, 005FCh, 004EEh>

; real opcodes: 0069Ch, 00870h, 00401h
header_VmCall EQU <DWORD marker1, marker2, 3h, 0062Ch, 008C8h, 004B7h>

; real opcodes: 00FCCh, 00C35h, 005CDh, 00D91h
header_VmJump EQU <DWORD marker1, marker2, 4h, 00F7Dh, 00C84h, 0057Ch, 00D28h>

; real opcodes: 00FC7h, 003D2h, 006FFh, 003BDh
header_VmAlloca EQU <DWORD marker1, marker2, 4h, 00F76h, 0036Bh, 0064Eh, 0030Ch>

; real opcodes: 003ECh, 00C5Ah, 00ECEh
header_VmHalt EQU <DWORD marker1, marker2, 3h, 0035Ch, 00CF2h, 00E7Eh>

; real opcodes: 009E7h, 00FB3h, 00513h
header_VmCmp EQU <DWORD marker1, marker2, 3h, 00955h, 00F09h, 005A9h>

; real opcodes: 00A1Bh, 0012Eh
header_VmStackWrite EQU <DWORD marker1, marker2, 2h, 00AB0h, 0019Dh>

; real opcodes: 00959h, 00C1Eh, 00EA0h, 0097Bh
header_VmStackRead EQU <DWORD marker1, marker2, 4h, 009F0h, 00CAFh, 00E19h, 009D2h>

; real opcodes: 00633h, 00DEBh, 00944h
header_VmJumpIfLess EQU <DWORD marker1, marker2, 3h, 00689h, 00D61h, 009F4h>

; real opcodes: 00BE4h, 00F8Ah, 00D58h
header_VmJumpIfLessEquals EQU <DWORD marker1, marker2, 3h, 00B54h, 00F42h, 00DF0h>

; real opcodes: 00F0Dh, 00744h, 00F37h
header_VmJumpIfGreater EQU <DWORD marker1, marker2, 3h, 00FBBh, 007F4h, 00F85h>

; real opcodes: 0027Dh, 00FA6h, 00A35h
header_VmJumpIfGreaterEquals EQU <DWORD marker1, marker2, 3h, 002CBh, 00F16h, 00A83h>

; real opcodes: 00409h, 00D7Fh, 00AEBh, 00824h, 00C31h
header_VmRead EQU <DWORD marker1, marker2, 5h, 004C1h, 00DCFh, 00A63h, 00896h, 00C89h>

; real opcodes: 00B64h, 004B5h, 008C8h
header_VmWrite EQU <DWORD marker1, marker2, 3h, 00BD4h, 00403h, 00880h>

; real opcodes: 00E5Fh, 002DCh, 0047Bh, 00671h
header_VmGetIp EQU <DWORD marker1, marker2, 4h, 00EEEh, 0026Dh, 004D2h, 006C8h>

; real opcodes: 00736h, 00413h, 00F35h
header_VmGetSp EQU <DWORD marker1, marker2, 3h, 00786h, 004A9h, 00F83h>

; real opcodes: 005F1h, 00DB9h, 0068Dh, 00D0Eh
header_VmNativeRead EQU <DWORD marker1, marker2, 4h, 00548h, 00D10h, 0063Ch, 00DBFh>

; real opcodes: 00F0Bh, 00AEEh
header_VmNativeWrite EQU <DWORD marker1, marker2, 2h, 00FC0h, 00A5Dh>

; real opcodes: 002A0h, 0007Ah, 0049Dh, 00AE1h, 00A72h
header_VmNativeCall EQU <DWORD marker1, marker2, 5h, 0021Ah, 000D4h, 0042Dh, 00A59h, 00ACCh>

; real opcodes: 0020Eh, 0055Ah, 00E9Dh, 003B2h, 00160h
header_VmByte EQU <DWORD marker1, marker2, 5h, 002C0h, 005F4h, 00E2Dh, 0030Ch, 001DAh>

; real opcodes: 00FECh, 00285h, 00498h, 00470h, 002A9h
header_VmWord EQU <DWORD marker1, marker2, 5h, 00F5Eh, 00235h, 00432h, 004CAh, 00221h>

; real opcodes: 00A85h, 00F78h, 008A6h
header_VmDoubleWord EQU <DWORD marker1, marker2, 3h, 00A33h, 00FD0h, 00816h>

; real opcodes: 0080Ah, 00C6Ah
header_VmSub EQU <DWORD marker1, marker2, 2h, 008C1h, 00CE1h>

; real opcodes: 00612h, 0044Eh
header_VmMul EQU <DWORD marker1, marker2, 2h, 006A9h, 004FDh>

; real opcodes: 00D85h, 0068Ah, 00922h, 0014Dh
header_VmDiv EQU <DWORD marker1, marker2, 4h, 00D34h, 00643h, 0099Bh, 001FCh>

; real opcodes: 008C1h, 00439h, 00435h
header_VmAnd EQU <DWORD marker1, marker2, 3h, 00877h, 0048Fh, 00483h>

; real opcodes: 003F6h, 00BE3h, 0026Bh, 00120h
header_VmShiftRight EQU <DWORD marker1, marker2, 4h, 00347h, 00B5Ah, 002E2h, 00199h>

; real opcodes: 00382h, 002BDh, 00687h, 009AFh
header_VmShiftLeft EQU <DWORD marker1, marker2, 4h, 0033Bh, 0020Ch, 00636h, 0091Eh>

; real opcodes: 0092Bh, 0021Ah, 00A6Eh
header_VmOr EQU <DWORD marker1, marker2, 3h, 009A1h, 002B2h, 00ADEh>

; real opcodes: 00490h, 00B80h, 00ED8h, 00B7Ch
header_VmNot EQU <DWORD marker1, marker2, 4h, 00429h, 00B39h, 00E71h, 00BCDh>

; real opcodes: 00831h, 00799h, 0046Ah, 002B2h
header_VmXor EQU <DWORD marker1, marker2, 4h, 00888h, 00730h, 004E3h, 0020Bh>

; real opcodes: 00870h, 00DCCh, 00820h, 007A5h
header_VmNor EQU <DWORD marker1, marker2, 4h, 008C9h, 00D7Dh, 00899h, 00714h>

; real opcodes: 00405h, 00BA4h, 00B2Eh, 00D36h
header_VmSetIp EQU <DWORD marker1, marker2, 4h, 004B4h, 00B15h, 00B9Fh, 00D87h>

; real opcodes: 0088Ah, 006ADh, 006D5h
header_VmSetSp EQU <DWORD marker1, marker2, 3h, 00842h, 0061Bh, 00663h>

; real opcodes: 00958h, 00682h, 001C7h, 0071Dh, 00860h
header_VmInc EQU <DWORD marker1, marker2, 5h, 009F2h, 0063Ch, 00177h, 007ADh, 008DAh>

header_marker EQU <DWORD marker2, marker1>
