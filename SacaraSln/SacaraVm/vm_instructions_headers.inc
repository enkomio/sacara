; This file is auto generated, don't modify it
marker1 EQU 094F48033h
marker2 EQU 0A5B094EFh

; real opcodes: 0E330h, 09624h, 07A24h, 0F143h, 0BC5Ah
header_VmRet EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 0E38Ah, 09696h, 07A96h, 0F1FBh, 0BCF4h>

; real opcodes: 0660h, 0DE68h, 095D7h
header_VmNop EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 066B8h, 0DEE0h, 09565h>

; real opcodes: 0D04Fh, 0FD10h, 093BBh
header_VmAdd EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0D0FDh, 0FDA8h, 09311h>

; real opcodes: 02272h, 09EEFh, 0F3B3h
header_VmPushImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 022CAh, 09E5Dh, 0F39h>

; real opcodes: 0E2B1h, 0B2AAh, 0D36h
header_VmPushVariable EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0E27h, 0B222h, 0D3B6h>

; real opcodes: 09A42h, 0FE41h, 0FD8Fh, 04E6Eh
header_VmPop EQU <DWORD 094F48033h, 0A5B094EFh, 4h, 09AFBh, 0FEF8h, 0FD3Eh, 04EDFh>

; real opcodes: 03B2h, 0A542h, 0470h
header_VmCall EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 03BBAh, 0A5FAh, 047B8h>

; real opcodes: 031D8h, 0C97h
header_VmNativeCall EQU <DWORD 094F48033h, 0A5B094EFh, 2h, 0316Fh, 0C9B4h>

; real opcodes: 0A2F5h, 03F8Ch
header_VmRead EQU <DWORD 094F48033h, 0A5B094EFh, 2h, 0A242h, 03F3Bh>

; real opcodes: 06DC1h, 02CC9h, 0578Ch, 099AAh
header_VmNativeRead EQU <DWORD 094F48033h, 0A5B094EFh, 4h, 06D78h, 02C80h, 0573Dh, 09923h>

; real opcodes: 09F43h, 03A1Eh, 0F222h
header_VmWrite EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 09FF9h, 03AAEh, 0F29Ah>

; real opcodes: 0487Ah, 023CAh, 01C47h
header_VmNativeWrite EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 048D2h, 02382h, 01CF5h>

; real opcodes: 087AEh, 023DEh, 0BA15h, 03FE0h, 0DC8h
header_VmGetIp EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 08720h, 02370h, 0BAA5h, 03F5Ah, 0D82h>

; real opcodes: 0FBE1h, 07BE2h, 0F3B2h
header_VmJumpImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0FB57h, 07B5Ah, 0F3Ah>

; real opcodes: 0AF3h, 02B7h, 0392Fh
header_VmJumpVariable EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0A49h, 02BB5h, 0399Dh>

; real opcodes: 0445Dh, 09E80h, 03413h
header_VmJumpIfLessImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 044EBh, 09E38h, 034A9h>

; real opcodes: 0CF42h, 0E2E4h, 0EAABh
header_VmJumpIfLessVariable EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0CFFAh, 0E254h, 0EA21h>

; real opcodes: 02A2Ch, 08B2Fh, 019Ah, 05580h
header_VmJumpIfLessEqualsImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 4h, 02A9Dh, 08B9Eh, 019C3h, 05539h>

; real opcodes: 0602Eh, 0C28h, 0933Bh, 05FDh, 03C18h
header_VmJumpIfLessEqualsVariable EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 060A0h, 0CA2h, 09393h, 054Dh, 03CB2h>

; real opcodes: 0C852h, 0BF50h
header_VmJumpIfGreatImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 2h, 0C8E9h, 0BFE7h>

; real opcodes: 077C2h, 08930h, 0ECDh, 04EA8h, 043AAh
header_VmJumpIfGreatVariable EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 0777Ch, 0898Ah, 0E7Dh, 04E22h, 04324h>

; real opcodes: 07ADh, 0BED2h
header_VmJumpIfGreatEqualsImmediate EQU <DWORD 094F48033h, 0A5B094EFh, 2h, 07ABAh, 0BE69h>

; real opcodes: 06EACh, 0446Bh, 0506Ch
header_VmJumpIfGreatEqualsVariable EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 06E1Ch, 044E1h, 050DCh>

; real opcodes: 053ABh, 03654h, 0D13Fh, 05486h, 03961h
header_VmAlloca EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 05323h, 036E6h, 0D18Fh, 05438h, 039D9h>

; real opcodes: 01BCh, 0F9B0h, 02EF0h, 026DFh, 0B847h
header_VmHalt EQU <DWORD 094F48033h, 0A5B094EFh, 5h, 01Eh, 0F9Ah, 02E4Ah, 0266Fh, 0B8F7h>

; real opcodes: 09BBDh, 06FC3h, 06081h
header_VmCmp EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 09BBh, 06F79h, 06037h>

; real opcodes: 0CED9h, 0C4Ch, 070DBh
header_VmGetSp EQU <DWORD 094F48033h, 0A5B094EFh, 3h, 0CE6Fh, 0CFCh, 07071h>

; real opcodes: 0589Eh, 026C8h, 0AAE1h, 06876h
header_VmStackWrite EQU <DWORD 094F48033h, 0A5B094EFh, 4h, 0582Fh, 02681h, 0AA58h, 068C7h>

; real opcodes: 09F44h, 0B24Ah, 0BF8h, 06B59h
header_VmStackRead EQU <DWORD 094F48033h, 0A5B094EFh, 4h, 09FF5h, 0B33h, 0B51h, 06BF0h>

header_end EQU <DWORD 0A5B094EFh, 094F48033h>
