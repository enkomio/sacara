; This file is auto generated, don't modify it
marker1 EQU 0CAC46B24h
marker2 EQU 0A89CB23Ah

; real opcodes: 0D707h, 035C4h, 00FA1h, 0DA61h
header_VmRet EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 0D7B6h, 03575h, 00F18h, 0DAD8h>

; real opcodes: 03446h, 0766Ah, 0F90Fh, 020E4h
header_VmNop EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 034F7h, 076E3h, 0F9BEh, 02055h>

; real opcodes: 0EA80h, 0A0C4h
header_VmAdd EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 0EA37h, 0A073h>

; real opcodes: 0C66Dh, 0CE5Fh
header_VmPushImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 0C6DAh, 0CEECh>

; real opcodes: 0887Fh, 003D2h, 0E9C1h, 0CF28h, 076CFh
header_VmPushVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 088CFh, 0036Ch, 0E979h, 0CFA2h, 0767Fh>

; real opcodes: 0C9F2h, 0C36Eh, 0892Fh, 0B405h, 03233h
header_VmPop EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 0C94Ch, 0C3E0h, 0899Fh, 0B4B5h, 0328Bh>

; real opcodes: 022E8h, 0E2E2h, 0D56Ah, 06295h, 08751h
header_VmCall EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 02262h, 0E25Ch, 0D5E4h, 06225h, 087E9h>

; real opcodes: 0CF82h, 08E37h, 013E2h
header_VmNativeCall EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 3h, 0CF3Ah, 08E85h, 0135Ah>

; real opcodes: 08A33h, 01987h, 0EE77h, 0AF6Ah, 007C3h
header_VmRead EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 08A8Bh, 01937h, 0EEC7h, 0AFE4h, 0077Bh>

; real opcodes: 067FAh, 0CD63h, 02857h, 03F15h
header_VmNativeRead EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 06753h, 0CDDAh, 028E6h, 03FA4h>

; real opcodes: 0927Ah, 04D25h, 07ABEh, 0F3E8h, 04E63h
header_VmWrite EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 092D4h, 04D95h, 07A10h, 0F362h, 04EDBh>

; real opcodes: 0ED32h, 06BD1h
header_VmNativeWrite EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 0ED89h, 06B66h>

; real opcodes: 0513Ch, 0DB1Eh, 0021Ch
header_VmGetIp EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 3h, 0518Ch, 0DBAEh, 002ACh>

; real opcodes: 0D630h, 03E00h, 037E6h, 087F4h
header_VmJumpImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 0D689h, 03EB9h, 03757h, 08745h>

; real opcodes: 09D9Eh, 0F6E9h
header_VmJumpVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 09D2Dh, 0F65Eh>

; real opcodes: 0DC52h, 0AB9Dh, 0799Eh
header_VmJumpIfLessImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 3h, 0DCEAh, 0AB2Bh, 0792Eh>

; real opcodes: 0E0AFh, 0C764h, 0E21Bh, 03EB8h, 05B7Eh
header_VmJumpIfLessVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 0E01Fh, 0C7D6h, 0E2B3h, 03E12h, 05BD0h>

; real opcodes: 08BB8h, 0F31Dh, 0E1C7h, 0906Dh
header_VmJumpIfLessEqualsImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 08B11h, 0F3ACh, 0E176h, 090DCh>

; real opcodes: 05AB6h, 06EABh, 038F8h, 05C4Bh
header_VmJumpIfLessEqualsVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 05A07h, 06E22h, 03851h, 05D02h>

; real opcodes: 02270h, 0C7AEh, 08BAEh, 0AF14h, 01775h
header_VmJumpIfGreatImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 022CAh, 0C720h, 08B20h, 0AFA6h, 017C5h>

; real opcodes: 0D938h, 0C9C1h, 05368h
header_VmJumpIfGreatVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 3h, 0D990h, 0C977h, 053E0h>

; real opcodes: 0B3EFh, 08AB7h, 0482Bh, 0B048h, 0B626h
header_VmJumpIfGreatEqualsImmediate EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 5h, 0B35Fh, 08A07h, 048A3h, 0B102h, 0B698h>

; real opcodes: 0012Ah, 023AEh, 06DB2h, 04C8Fh
header_VmJumpIfGreatEqualsVariable EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 001A3h, 0231Fh, 06D0Bh, 04C3Eh>

; real opcodes: 02D23h, 0F0B3h
header_VmAlloca EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 02D98h, 0F008h>

; real opcodes: 04F6Fh, 00EFDh, 0C267h, 045CDh
header_VmHalt EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 04FDEh, 00E4Ch, 0C2D6h, 0457Ch>

; real opcodes: 0DD2Fh, 01952h, 0E084h, 0415Dh
header_VmCmp EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 0DD9Eh, 019EBh, 0E035h, 041ECh>

; real opcodes: 01DCCh, 014AEh
header_VmGetSp EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 01D7Bh, 0141Dh>

; real opcodes: 0CA88h, 0B8D8h
header_VmStackWrite EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 2h, 0CA3Fh, 0B86Fh>

; real opcodes: 0EE2Ah, 01863h, 0EB4Eh, 07B79h
header_VmStackRead EQU <DWORD 0CAC46B24h, 0A89CB23Ah, 4h, 0EEA3h, 018DAh, 0EBFFh, 07BD0h>

header_end EQU <DWORD 0A89CB23Ah, 0CAC46B24h>
