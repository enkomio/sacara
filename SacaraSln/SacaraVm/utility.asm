; *****************************
; arguments: size to alloc
; *****************************
heap_alloc PROC
	push ebp
	mov ebp, esp
	sub esp, 8

	; allocate space in the heap for the stack of the entry function
	push hash_kernel32_dll
	call find_module_base
	mov [ebp+local0], eax ; save kernel32 base

	push hash_ntdll_dll
	call find_module_base
	mov [ebp+local1], eax ; save ntdll base
	
	push hash_GetProcessHeap
	push [ebp+local0]
	call find_exported_func
	
	; call GetProcessHeap
	call eax

	; push HeapAlloc arguments
	push [ebp+arg0] ; size
	push HEAP_ZERO_MEMORY
	push eax ; process heap

	; resolve function
	push hash_RtlAllocateHeap
	push [ebp+local1]
	call find_exported_func

	; call HeapAlloc
	call eax 

	add esp, 8
	mov ebp, esp
	pop ebp
	ret 4h
heap_alloc ENDP

; *****************************
; arguments: memory
; *****************************
heap_free PROC
	push ebp
	mov ebp, esp
	sub esp, 8

	; find kernel32
	push hash_kernel32_dll
	call find_module_base
	mov [ebp+local0], eax ; save kernel32 base

	; find ntdll
	push hash_ntdll_dll
	call find_module_base
	mov [ebp+local1], eax ; save ntdll base
	
	; call GetProcessHeap
	push hash_GetProcessHeap
	push [ebp+local0]
	call find_exported_func
	call eax

	; push HeapAlloc arguments
	push [ebp+arg0] ; addr to free
	push 0h ; flag
	push eax ; process heap

	; call RtlFreeHeap
	push hash_RtlFreeHeap
	push [ebp+local1]
	call find_exported_func
	call eax
	
	add esp, 8
	mov ebp, esp
	pop ebp
	ret 04h
heap_free ENDP

; *****************************
; arguments: start_memory, size, marker
; *****************************
find_vm_handler PROC
	push ebp
	mov ebp, esp
	mov edi, [ebp+arg0] ; memory
	mov ecx, [ebp+arg1] ; size		

search_header_loop:
	; search the first marker
	cmp dword ptr [edi], marker1
	je first_marker_found
	inc edi
	loopne search_header_loop
	jne not_found	
	
first_marker_found:
	; check the second hardcoded marker
	add edi, 4
	cmp dword ptr [edi], marker2
	jne search_header_loop	

	; function header found, read the number of supported opcode
	mov eax, edi
	add eax, 4
	push ecx
	mov edx, [eax] ; read the number of possible opcodes
	mov ecx, edx
	
	; search for the given opcode
	mov esi, [ebp+arg2] ; opcode to search for

	; decode the opcode
	xor esi, 0B5h
	add si, cx
search_opcode_loop:	
	add eax, 4
	cmp dword ptr [eax], esi
	loopne search_opcode_loop

	; jump if this is not the header that we are searching for	
	pop ecx
	jne search_header_loop

	; function found, save the read address in EAX
	; EDI contains the address of marker2
	lea eax, [edi+TYPE DWORD*edx+8]
	jmp found

not_found:
	xor eax, eax
found:
	mov ebp, esp
	pop ebp
	ret 0Ch
find_vm_handler ENDP

; *****************************
; arguments: memory, size
; *****************************
hash_string proc
	push ebp
	mov ebp, esp
	
	xor eax, eax
	mov esi, [ebp+arg0] ; memory
	mov ecx, [ebp+arg1] ; size
		
hash_loop: 
	cmp byte ptr [esi], 0
	je loop_epilogue

	; hash * memory[i]
	mov edx, eax
	movzx ebx, byte ptr [esi]

	; to uppercase if needed
	cmp ebx, 'a'
	jb hash_iteration
	cmp ebx, 'z'
	ja hash_iteration
	lea ebx, [ebx-20h]

hash_iteration:
	add edx, ebx
	push eax
	mov eax, edx
	mov edx, 400h
	mul edx
	mov edx, eax
	pop eax

	mov ebx, edx
	ror ebx, 6
	xor edx, ebx
	mov eax, edx

loop_epilogue:
	inc esi	
	loop hash_loop

exit:
	mov ebp, esp
	pop ebp
	ret 8
hash_string endp


; *****************************
; arguments: modulo hash
; *****************************
find_module_base proc
	push ebp
	mov ebp, esp

	sub esp, 8h

	assume fs:nothing
	mov eax, fs:[30h]  ; PEB
	assume fs:error
	
	; get head module list
	mov eax, [eax+0ch] ; Ldr
	mov eax, [eax+14h] ; InMemoryOrderModuleList entry
	mov [ebp+local0], eax ; head
	mov [ebp+local1], eax ; cur entry

find_module_loop:
	; get module name	
	lea edi, [eax+1Ch] ; UNICODE_STRING FullDllName

	; find the last '/' character
	movzx ecx, word ptr [edi] ; get the string length in bytes
	lea edi, [edi+4] ; point to the unicode buffer
	mov edi, [edi]
	add edi, ecx ; point to the end of the string
	std ; scan backward

	; scan memory for char
	xor eax, eax
	mov al, 5ch
	mov esi, edi
	repnz scasb ; scan to find the char '\'
	cld
	jnz compute_length
	inc edi
	inc edi
compute_length:
	sub esi, edi ; compute module basename length

	; compute hash of the module base name
	push esi
	push edi
	call hash_string
	
	; check if module hash is equals
	cmp eax, [ebp+arg0]
	je module_found
	
	mov eax, [ebp+local1] ; cur entry
	mov eax, [eax] ; go to next entry
	mov [ebp+local1], eax
	cmp [ebp+local0], eax
	jne find_module_loop
	xor eax, eax
	jmp module_not_found

module_found:
	mov eax, [ebp+local1] ; cur entry
	mov eax, [eax+10h] ; DllBase

module_not_found:
	add esp, 8h

	mov ebp, esp
	pop ebp
	ret 4
find_module_base endp

; *****************************
; arguments: dll base, function name hash
; *****************************
find_exported_func PROC
	push ebp
	mov ebp, esp

	sub esp, 8
	
	; check MZ signature
	mov ebx, [ebp+arg0] ; DLL base
	mov ax, word ptr [ebx]
	cmp ax, 'ZM'
	jnz error

	; check PE signature
	lea ebx, [ebx+03Ch]
	mov ebx, [ebx]
	add ebx, [ebp+arg0]
	mov ax, word ptr [ebx]
	cmp ax, 'EP'
	jnz error
	
	; go to Export table address
	mov edx, [ebx+078h] ; Virtual Address
	test edx, edx
	jz error

	; add base address to compute IMAGE_EXPORT_DIRECTORY
	add edx, [ebp+arg0]
	mov ecx, [edx+014h] ; NumberOfFunctions

	; save index
	xor eax, eax
	mov [ebp+local0], edx ; save IMAGE_EXPORT_DIRECTORY
	mov [ebp+local1], eax  ; save index

function_name_loop:
	mov edx, [ebp+local0] ; IMAGE_EXPORT_DIRECTORY
	mov ebx, [ebp+local1] ; index

	; get the i-th function name
	mov esi, [edx+020h] ; AddressOfNames RVA
	add esi, [ebp+arg0] ; AddressOfNames VA
	lea esi, [esi+TYPE DWORD*ebx] ; point to the current index	
	mov edi, [esi] ; function name RVA
	add edi, [ebp+arg0] ; function name VA
	
	; scan to find the NULL char
	push eax
	xor eax, eax	
	mov esi, edi
	repnz scasb
	pop eax

	; compute name length
	sub edi, esi
	dec edi

	; compute function name hash
	push ecx
	push edi
	push esi
	call hash_string
	pop ecx

	; compare hash
	cmp eax, [ebp+arg1]
	je function_name_found

	; go to next name pointer
	inc dword ptr [ebp+local1]

	loop function_name_loop
	jmp error

function_name_found:
	mov edx, [ebp+local0] ; IMAGE_EXPORT_DIRECTORY
	mov ebx, [ebp+local1] ; index

	; get the i-th function ordinal
	mov esi, [edx+024h] ; AddressOfNameOrdinals RVA
	add esi, [ebp+arg0] ; AddressOfNameOrdinals VA
	lea esi, [esi+TYPE WORD*ebx] ; point to the current index	
	movzx eax, word ptr [esi] ; ordinal
		
	; get the i-th function address
	mov esi, [edx+01Ch] ; AddressOfFunctions RVA
	add esi, [ebp+arg0] ; AddressOfFunctions VA
	lea esi, [esi+TYPE DWORD*eax] ; point to the current index	
	mov eax, [esi] ; function addr RVA
	add eax, [ebp+arg0] ; function addr VA
	jmp finish

error:
	xor eax, eax
	
finish:
	add esp, 8
	mov ebp, esp
	pop ebp
	ret 8
find_exported_func ENDP