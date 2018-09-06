#include <stdio.h>
#include <stdint.h>
#include <Windows.h>

void main()
{
	uint8_t code[] = {
		0x25,0x4,0x1,0x0,0x0,0x0,                // /* 250401000000 */ loc_00000000: VmPushImmediate 0x1
		0x18,0xF,                                // /*         180F */ loc_00000006: VmAlloca
		0x25,0x4,0xC,0x0,0x0,0x0,                // /* 25040C000000 */ loc_00000008: VmPushImmediate 0xC
		0x25,0x4,0x2C,0x0,0x0,0x0,               // /* 25042C000000 */ loc_0000000E: VmPushImmediate 0x2C
		0xC9,0x1,                                // /*         C901 */ loc_00000014: VmCmp
		0x25,0x4,0x28,0x0,0x0,0x0,               // /* 250428000000 */ loc_00000016: VmPushImmediate 0x28
		0xC7,0x8,                                // /*         C708 */ loc_0000001C: VmJumpIfLess
		0x25,0x4,0x1,0x0,0x0,0x0,                // /* 250401000000 */ loc_0000001E: VmPushImmediate 0x1
		0x8A,0x9,0x0,0x0,                        // /*     8A090000 */ loc_00000024: VmPop 0x0
		0x25,0x4,0x0,0x0,0x0,0x0,                // /* 250400000000 */ loc_00000028: VmPushImmediate 0x0
		0x8A,0x9,0x0,0x0,                        // /*     8A090000 */ loc_0000002E: VmPop 0x0
		0xED,0x6                                 // /*         ED06 */ loc_00000032: VmHalt
	};
	printf("Hello world!");
}