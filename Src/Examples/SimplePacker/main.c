#include <stdint.h>
#include <stdio.h>
#include <Windows.h>

extern uint32_t __stdcall vm_init(uint8_t[], uint32_t);
extern uint32_t __stdcall vm_run(uint32_t);
extern void __stdcall vm_set_error_handler(uint32_t, uint32_t);
extern void __stdcall vm_local_var_set(uint32_t, uint32_t, uint32_t);
extern uint32_t __stdcall vm_local_var_get(uint32_t, uint32_t);
extern void __stdcall vm_free(uint32_t);

static char payload_name[] = "DATA";
static char password_name[] = "SECRET";
static char sacara_name[] = "SACARA";

static void *read_resource(char *name, uint32_t *res_size) 
{
	HRSRC res_info = FindResource(NULL, name, MAKEINTRESOURCE(RT_RCDATA));
	if (!res_info) return NULL;
	
	HGLOBAL hres = LoadResource(NULL, res_info);
	if (!hres) return NULL;

	void *res = LockResource(hres);
	if (!res) return NULL;

	*res_size = SizeofResource(NULL, res_info);
	void *buffer = VirtualAlloc(NULL, *res_size, MEM_COMMIT, PAGE_EXECUTE_READWRITE);	
	memcpy(buffer, res, *res_size);
	if (FreeResource(hres)) return NULL;

	return buffer;
}

static uint32_t exec_vm_code(
	uint8_t *data, 
	uint32_t data_size, 
	uint8_t *password,
	uint32_t password_size,
	uint8_t *vm_code, 
	uint32_t vm_code_size
)
{
	uint32_t vm_context, result = 0;	
	vm_context = vm_init(vm_code, vm_code_size);
	vm_local_var_set(vm_context, 0, (uint32_t)data);
	vm_local_var_set(vm_context, 1, data_size);
	vm_local_var_set(vm_context, 2, (uint32_t)password);
	vm_local_var_set(vm_context, 3, password_size);
	
	// run the code
	result = vm_run(vm_context);
	vm_free(vm_context);	
	return result;
}

int main()
{
	// execute code
	uint8_t *data = NULL;
	uint32_t data_size;
	uint8_t *password = NULL;
	uint32_t password_size;
	uint8_t *vm_code = NULL;
	uint32_t vm_code_size;

	// get resource content	
	data = read_resource(payload_name, &data_size);
	if (!data) goto complete;

	// get the encrypted password. This is XOR obfuscated with 0xA1
	password = read_resource(password_name, &password_size);
	if (!password) goto complete;

	// get vm code content	
	vm_code = read_resource(sacara_name, &vm_code_size);
	if (!vm_code) goto complete;
	
	// run vm the code
	uint32_t vm_result = exec_vm_code(
		data, 
		data_size, 
		password, 
		password_size, 
		vm_code, 
		vm_code_size
	);
	
	if (vm_result)
		printf("Sacara VM code execution encountered an error at offset: %d\n", vm_result);

complete:
	if (data) VirtualFree(data, data_size, MEM_DECOMMIT);
	if (password) VirtualFree(password, password_size, MEM_DECOMMIT);
	if (vm_code) VirtualFree(vm_code, vm_code_size, MEM_DECOMMIT);
	
	return 0;
}
