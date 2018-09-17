#include <stdio.h>
#include <stdint.h>
#include <Windows.h>

// this code was generated with the Sacara assembler. See test.sacara for the original script.
uint8_t code[] = {
	0xCF,0x4E,0xC5,0x37,0x72,0x52,           // /* CF4EC5377252 */ loc_00000000: VmPushImmediate 0x527237C5
	0xC7,0xF,                                // /* C70F         */ loc_00000006: VmAlloca 
	0xCF,0x4E,0xCE,0x3F,0x7A,0x5A,           // /* CF4ECE3F7A5A */ loc_00000008: VmPushImmediate 0x5A7A3FCE
	0xCF,0x4E,0x9A,0x25,0x60,0x40,           // /* CF4E9A256040 */ loc_0000000E: VmPushImmediate 0x4060259A
	0x9C,0x6,                                // /* 9C06         */ loc_00000014: VmCall 
	0xFD,0x4D,0x4A,0xD0,                     // /* FD4D4AD0     */ loc_00000016: VmPushVariable 0xD04A
	0xFD,0x4D,0x4F,0xD4,                     // /* FD4D4FD4     */ loc_0000001A: VmPushVariable 0xD44F
	0xCF,0x4E,0xE6,0x15,0x50,0x70,           // /* CF4EE6155070 */ loc_0000001E: VmPushImmediate 0x705015E6
	0xCF,0x4E,0xAA,0x1B,0x5E,0x7E,           // /* CF4EAA1B5E7E */ loc_00000024: VmPushImmediate 0x7E5E1BAA
	0x9C,0x6,                                // /* 9C06         */ loc_0000002A: VmCall 
	0xCF,0x4E,0xF2,0x3,0x46,0x66,            // /* CF4EF2034666 */ loc_0000002C: VmPushImmediate 0x664603F2
	0xCF,0x4E,0xF9,0x9,0x4C,0x6C,            // /* CF4EF9094C6C */ loc_00000032: VmPushImmediate 0x6C4C09F9
	0xFD,0x4D,0x6E,0xF6,                     // /* FD4D6EF6     */ loc_00000038: VmPushVariable 0xF66E
	0xA0,0x2,                                // /* A002         */ loc_0000003C: VmNativeCall 
	0xEC,0x3,                                // /* EC03         */ loc_0000003E: VmHalt 
	0xB8,0xAC,                               // /* 1206         */ loc_00000040: VmMul (ENC)
	0x65,0xE4,0x9C,0x69,0x68,0x7C,           // /* CF4E36C3C2D6 */ loc_00000042: VmPushImmediate 0xD6C2C336 (ENC)
	0x9B,0xA2,                               // /* 3108         */ loc_00000048: VmXor (ENC)
	0x30,0xA3,                               // /* 9A09         */ loc_0000004A: VmRet (ENC)
	0x70,0xD,                                // /* 700D         */ loc_0000004C: VmNop 
	0xCF,0x4E,0x95,0x65,0x20,0x0,            // /* CF4E95652000 */ loc_0000004E: VmPushImmediate 0x206595
	0xC7,0xF,                                // /* C70F         */ loc_00000054: VmAlloca 
	0xCF,0x4E,0xDC,0x6D,0x28,0x8,            // /* CF4EDC6D2808 */ loc_00000056: VmPushImmediate 0x8286DDC
	0x2,0x4B,0x30,0xAA,                      // /* 024B30AA     */ loc_0000005C: VmPop 0xAA30
	0xFD,0x4D,0x34,0xAE,                     // /* FD4D34AE     */ loc_00000060: VmPushVariable 0xAE34
	0x9,0x4,                                 // /* 0904         */ loc_00000064: VmRead 
	0xCF,0x4E,0x6,0x5D,0x18,0x38,            // /* CF4E065D1838 */ loc_00000066: VmPushImmediate 0x38185D06
	0x31,0x8,                                // /* 3108         */ loc_0000006C: VmXor 
	0xFD,0x4D,0x22,0xB8,                     // /* FD4D22B8     */ loc_0000006E: VmPushVariable 0xB822
	0x64,0xB,                                // /* 640B         */ loc_00000072: VmWrite 
	0xFD,0x4D,0x28,0xB2,                     // /* FD4D28B2     */ loc_00000074: VmPushVariable 0xB228
	0xCF,0x4E,0xBF,0x4F,0xA,0x2A,            // /* CF4EBF4F0A2A */ loc_00000078: VmPushImmediate 0x2A0A4FBF
	0xE,0xB,                                 // /* 0E0B         */ loc_0000007E: VmAdd 
	0x2,0x4B,0xD4,0x4E,                      // /* 024BD44E     */ loc_00000080: VmPop 0x4ED4
	0xFD,0x4D,0xD8,0x42,                     // /* FD4DD842     */ loc_00000084: VmPushVariable 0x42D8
	0xCF,0x4E,0x2,0xBF,0xFA,0xDA,            // /* CF4E02BFFADA */ loc_00000088: VmPushImmediate 0xDAFABF02
	0xE7,0x9,                                // /* E709         */ loc_0000008E: VmCmp 
	0xCF,0x4E,0x36,0xA7,0xE2,0xC2,           // /* CF4E36A7E2C2 */ loc_00000090: VmPushImmediate 0xC2E2A736
	0xD,0xF,                                 // /* 0D0F         */ loc_00000096: VmJumpIfGreat 
	0x9A,0x9                                 // /* 9A09         */ loc_00000098: VmRet 
};

typedef struct _vm_context {
	uint32_t *ip;
	uint32_t *stack;
	uint32_t status_flag;
	uint32_t *code;
	uint32_t code_size;
} vm_context;

typedef void(__stdcall *vm_init_func)(vm_context*, uint8_t[], uint32_t);
typedef uint32_t(__stdcall *vm_run_func)(vm_context*);
typedef void(__stdcall *vm_free_func)(vm_context*);
typedef void(__stdcall *vm_local_var_set_func)(vm_context*, uint32_t, uint32_t);
typedef uint32_t(__stdcall *vm_local_var_get_func)(vm_context*, uint32_t);

// VM functions
vm_init_func vm_init = NULL;
vm_run_func vm_run = NULL;
vm_free_func vm_free = NULL;
vm_local_var_set_func vm_local_var_set = NULL;
vm_local_var_get_func vm_local_var_get = NULL;

void resolve_vm_functions()
{
	HMODULE hModule = NULL;
	hModule = LoadLibrary("SacaraVm.dll");
	vm_init = (vm_init_func)GetProcAddress(hModule, "vm_init");
	vm_run = (vm_run_func)GetProcAddress(hModule, "vm_run");
	vm_free = (vm_free_func)GetProcAddress(hModule, "vm_free");
	vm_local_var_set = (vm_local_var_set_func)GetProcAddress(hModule, "vm_local_var_set");
	vm_local_var_get = (vm_local_var_get_func)GetProcAddress(hModule, "vm_local_var_get");
}

void print_result(uint32_t result)
{
	printf("You are %x :)", result);
}

int main()
{
	vm_context ctx = { 0 };
	uint32_t result = 0;
	
	resolve_vm_functions();

	// initialize the VM context structure
	vm_init(&ctx, code, sizeof(code));

	// add first parameter
	vm_local_var_set(&ctx, 0, 158911);

	// add second parameter
	vm_local_var_set(&ctx, 1, 21431);

	// add native method to print result
	vm_local_var_set(&ctx, 2, (uint32_t)print_result);

	// run the code
	result = vm_run(&ctx);

	// free the VM
	vm_free(&ctx);

	return result;
}