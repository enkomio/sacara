; This file is auto generated, don't modify it
marker1 EQU 02E587B90h
marker2 EQU 026981C2Fh

; real opcodes: 0DE8Eh, 07660h
header_VmRet EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 0DE3Dh, 076D7h>

; real opcodes: 07E04h, 07FCEh
header_VmNop EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 07EB3h, 07F7Dh>

; real opcodes: 0736Fh, 08B5Dh, 08B12h, 08937h
header_VmAdd EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 073DEh, 08BECh, 08BABh, 08986h>

; real opcodes: 0BD37h, 08B3Eh, 0563Ch
header_VmPushImmediate EQU <DWORD 02E587B90h, 026981C2Fh, 3h, 0BD85h, 08B8Eh, 0568Ch>

; real opcodes: 07A3Bh, 096E4h, 0007Ch, 03F94h, 0DC0Dh
header_VmPushVariable EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 07A93h, 09656h, 000CEh, 03F26h, 0DCBDh>

; real opcodes: 0A913h, 0AFB0h, 0A53Bh, 04FFFh
header_VmPop EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 0A9AAh, 0AF09h, 0A592h, 04F4Eh>

; real opcodes: 050E4h, 022E6h, 0E5BAh, 04945h
header_VmCall EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 05055h, 02257h, 0E513h, 049F4h>

; real opcodes: 0F3F3h, 0A0B0h, 08B68h, 00480h
header_VmNativeCall EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 0F34Ah, 0A009h, 08BE1h, 00439h>

; real opcodes: 061CFh, 0E697h, 0E434h, 00546h, 082CFh
header_VmRead EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 0617Fh, 0E627h, 0E486h, 005F8h, 0827Fh>

; real opcodes: 075FBh, 098F8h, 00E56h, 05941h
header_VmNativeRead EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 07552h, 09851h, 00EE7h, 059F8h>

; real opcodes: 07487h, 0312Dh, 08663h, 0ABB2h
header_VmWrite EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 07436h, 0319Ch, 086DAh, 0AB0Bh>

; real opcodes: 0E026h, 01939h, 0770Bh, 03584h
header_VmNativeWrite EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 0E097h, 01990h, 077C2h, 03535h>

; real opcodes: 05C2Ch, 02E0Fh, 0EE4Fh, 03670h, 05092h
header_VmGetIp EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 05C9Eh, 02EBFh, 0EEFFh, 036CAh, 0502Ch>

; real opcodes: 041D7h, 0DD4Fh, 08B45h, 01C91h
header_VmJump EQU <DWORD 02E587B90h, 026981C2Fh, 4h, 04166h, 0DDFEh, 08BF4h, 01C28h>

; real opcodes: 051C6h, 002C1h, 01875h, 0B04Bh, 01C47h
header_VmJumpIfLess EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 05178h, 00279h, 018C5h, 0B103h, 01CF7h>

; real opcodes: 05948h, 000D7h
header_VmJumpIfLessEquals EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 059FFh, 00064h>

; real opcodes: 016C2h, 096EFh
header_VmJumpIfGreat EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 01679h, 0965Ch>

; real opcodes: 00D5Dh, 0DC5Ch, 0F1B7h, 05F60h, 09D91h
header_VmJumpIfGreatEquals EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 00DEDh, 0DCEEh, 0F107h, 05FDAh, 09D29h>

; real opcodes: 013D7h, 08780h
header_VmAlloca EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 01364h, 08737h>

; real opcodes: 0CEFAh, 00C29h
header_VmHalt EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 0CE51h, 00C9Eh>

; real opcodes: 0B892h, 0F5E3h
header_VmCmp EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 0B829h, 0F558h>

; real opcodes: 03246h, 0E9A9h, 0342Eh, 0BBA7h, 0830Fh
header_VmGetSp EQU <DWORD 02E587B90h, 026981C2Fh, 5h, 032F8h, 0E921h, 034A0h, 0BB17h, 083BFh>

; real opcodes: 08454h, 022CAh
header_VmStackWrite EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 084E3h, 02281h>

; real opcodes: 06DF7h, 0FB66h
header_VmStackRead EQU <DWORD 02E587B90h, 026981C2Fh, 2h, 06D44h, 0FBD5h>

header_end EQU <DWORD 026981C2Fh, 02E587B90h>
