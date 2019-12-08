#include <stdio.h>
#include <stdint.h>
#include <Windows.h>
#include "argparse.h"

// VM functions
extern uint32_t __stdcall vm_init(uint8_t[], uint32_t);
extern uint32_t __stdcall vm_run(uint32_t);
extern void __stdcall vm_set_error_handler(uint32_t, uint32_t);
extern void __stdcall vm_local_var_set(uint32_t, uint32_t, uint32_t);
extern uint32_t __stdcall vm_local_var_get(uint32_t, uint32_t);
extern void __stdcall vm_free(uint32_t);

static const char *const usage[] = {
	"SacaraRun.exe <file>",
	NULL
};

uint32_t run_code(char* filename, int argc, const char** argv)
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
				uint32_t vm_context, result = 0;
				vm_context = vm_init(file_content, file_size);
				
				// set the arguments
				for (uint32_t i = 0; i < argc; i++)
				{
					uint32_t num = (uint32_t)strtol(argv[i], NULL, 10);
					vm_local_var_set(vm_context, i, num);
				}

				// run the code
				printf("Execute file: %s\n", filename);
				result = vm_run(vm_context);				
								
				if (result == 0)
				{
					uint32_t execution_result = vm_local_var_get(vm_context, 0);
					printf("Code execution result: %d\n", execution_result);
				}
				else
				{
					printf("An error was generated at offset: %d\n", result);
				}

				// free the VM
				vm_free(vm_context);
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
	argparse_describe(&argparse, "\nSacaraRun allows to execute Sacara Intermediate Language (SIL) code", "\nUsage: sacara.exe <OPTIONS> [arg0|arg1|...]");
	argc = argparse_parse(&argparse, argc, argv);

	if (path != NULL)
	{
		result = run_code(path, argc, argv);
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