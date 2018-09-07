#include <stdio.h>
#include <stdint.h>
#include <Windows.h>
#include "argparse.h"

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

static const char *const usage[] = {
	"SacaraRun.exe <file>",
	NULL
};

uint32_t run_code(char* filename)
{
	uint32_t result = -1;	
	HANDLE hFile = NULL;	

	hFile = CreateFile(filename, GENERIC_READ, 0, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
	if (hFile != INVALID_HANDLE_VALUE)
	{
		uint32_t file_size, *count;
		uint8_t *file_content = NULL;

		file_size = GetFileSize(hFile, NULL);
		file_content = HeapAlloc(GetProcessHeap(), HEAP_ZERO_MEMORY, file_size);
		if (file_content != NULL)
		{
			if (ReadFile(hFile, file_content, file_size, &count, NULL))
			{
				vm_context ctx = { 0 };
				uint32_t execution_result;

				resolve_vm_functions();

				// initialize the VM context structure
				vm_init(&ctx, file_content, file_size);

				// run the code
				result = vm_run(&ctx);

				if (result == 0)
				{
					execution_result = vm_local_var_get(&ctx, 0);
					printf("Code execution result: %d", execution_result);
				}

				// free the VM
				vm_free(&ctx);
			}

			HeapFree(GetProcessHeap(), NULL, file_content);
		}
	}	

	return result;
}

int main(int argc, const char** argv)
{	
	uint32_t result = 0;
	char *path = NULL;

	// parse arguments
	struct argparse_option options[] = {
		OPT_HELP(),
		OPT_GROUP("Basic options"),
		OPT_STRING('p', "path", &path, "source file"),
		OPT_END(),
	};

	struct argparse argparse;
	argparse_init(&argparse, options, usage, 0);
	argparse_describe(&argparse, "\nSacaraRun allows to execute Sacara Intermediate Language (SIL) code", NULL);
	argc = argparse_parse(&argparse, argc, argv);

	if (path != NULL)
	{
		result = run_code(path);
		if (result)
		{
			printf("An error was encountered during the code execution. Error at offset: %d", result);
		}
	}		
	else
	{
		argparse_usage(&argparse);
	}		

	return result;
}