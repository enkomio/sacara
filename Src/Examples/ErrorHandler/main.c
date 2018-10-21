/**

This example show how to invoke the error handler routine in case of error. The handler return a boolean which tell if the
exception can continue after the execution or not.
*/

#include <stdbool.h>
#include <stdio.h>
#include <stdint.h>
#include <tchar.h>
#include <Windows.h>

uint8_t code[] = {
	0xCF, 0x0E, 0x01, 0x00, 0x00, 0x00, // VmPushImmediate 0x1
	0xC7, 0x0F, // VmAlloca
	0xFF, 0xAA, // invalid instruction
	0xCF, 0x0E, 0x09, 0x00, 0x00, 0x00, // VmPushImmediate 0x9
	0xCF, 0x0E, 0x06, 0x00, 0x00, 0x00, // VmPushImmediate 0x6
	0xCF, 0x0E, 0x02, 0x00, 0x00, 0x00, // VmPushImmediate 0x2
	0xCF, 0x0E, 0x2A, 0x00, 0x00, 0x00, // VmPushImmediate 0x2A
	0x9C, 0x06, // VmCall
	0x02, 0x0B, 0x00, 0x00, // VmPop 0x0
	0xEC, 0x03, // VmHalt
	0x0E, 0x0B, // VmAdd
	0x9A, 0x09 // VmRet
};

typedef uint32_t(__stdcall *vm_init_func)(uint8_t[], uint32_t);
typedef uint32_t(__stdcall *vm_run_func)(uint32_t);
typedef void(__stdcall *vm_free_func)(uint32_t);
typedef void(__stdcall *vm_set_error_handler_func)(uint32_t, uint32_t);
typedef uint32_t(__stdcall *vm_local_var_get_func)(uint32_t, uint32_t);

// VM functions
vm_init_func vm_init = NULL;
vm_run_func vm_run = NULL;
vm_free_func vm_free = NULL;
vm_local_var_get_func vm_local_var_get = NULL;
vm_set_error_handler_func vm_set_error_handler = NULL;

void resolve_vm_functions()
{
	HMODULE hModule = NULL;
	hModule = LoadLibrary("SacaraVm.dll");
	if (!hModule)
	{
		uint32_t error_code = GetLastError();

		LPTSTR errorText = NULL;

		FormatMessage(
			FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_IGNORE_INSERTS,
			NULL,
			error_code,
			MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),
			(LPTSTR)&errorText,
			0,
			NULL
		);

		if (NULL != errorText)
		{
			_tprintf(errorText);
			LocalFree(errorText);
		}
	}
	else
	{
		vm_init = (vm_init_func)GetProcAddress(hModule, "vm_init");
		vm_run = (vm_run_func)GetProcAddress(hModule, "vm_run");
		vm_free = (vm_free_func)GetProcAddress(hModule, "vm_free");
		vm_local_var_get = (vm_local_var_get_func)GetProcAddress(hModule, "vm_local_var_get");
		vm_set_error_handler = (vm_set_error_handler_func)GetProcAddress(hModule, "vm_set_error_handler");
	}
}

bool handle_ignore_error(uint32_t state, uint32_t error_code)
{
	if (error_code == 0x0FF000001)
	{
		printf("Error during the execution of code at offset: %d. Ignore IT.\n", state);
	}	
	return true; // ignore the error and continue execution
}

bool handle_error(uint32_t state, uint32_t error_code)
{
	if (error_code == 0x0FF000001)
	{
		printf("Error during the execution of code at offset: %d. End execution.\n", state);
	}
	return false; // end the execution of the code
}

uint32_t test(uint32_t func_ptr)
{
	uint32_t result = 0, vm_handle;

	resolve_vm_functions();

	// initialize the VM context structure
	vm_handle = vm_init(code, sizeof(code));

	// set the error handler
	vm_set_error_handler(vm_handle, func_ptr);

	// run the buggy code
	result = vm_run(vm_handle);

	// get the result addition
	result = vm_local_var_get(vm_handle, 0);

	// free the VM
	vm_free(vm_handle);

	return result;
}

void main()
{
	uint32_t result = 0;
	
	// first test ignore error
	result = test((uint32_t)handle_ignore_error);
	printf("First code execution returned: %d\n", result);

	// second test terminate on error
	result = 0;
	result = test((uint32_t)handle_error);
	printf("Second code execution returned: %d\n", result);
}