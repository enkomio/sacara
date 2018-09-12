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

	mov esp, ebp
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
	
	mov esp, ebp
	pop ebp
	ret 04h
heap_free ENDP

; *****************************
; arguments: start_memory, size, opcode
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
	push ecx ; save outer loop counter
	mov edx, [eax] ; read the number of possible opcodes
	mov ecx, edx ; set the loop counter
	
	; search for the given opcode
	mov esi, [ebp+arg2] ; opcode to search for	
	xor esi, INIT_OPCODE_HEADER_XOR_KEY
	add si, dx
	
search_opcode_loop:	
	add eax, 4
	cmp [eax], esi
	loopne search_opcode_loop

	; jump if this is not the header that we are searching for	
	pop ecx ; restore outer loop counter
	jne search_header_loop

	; function found, save the read address in EAX
	; EDI contains the address of marker2
	lea eax, [edi+TYPE DWORD*edx+8]
	jmp found

not_found:
	xor eax, eax
found:
	mov esp, ebp
	pop ebp
	ret 0Ch
find_vm_handler ENDP

; *****************************
; arguments: memory, size
; *****************************
hash_string proc
	push ebp
	mov ebp, esp
	sub esp, 4
	
	xor eax, eax
	mov esi, [ebp+arg0] ; memory
	mov ecx, [ebp+arg1] ; size
	mov dword ptr [ebp+local0], 0h
		
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
	and bl, 11011111b ; clar bit 5

hash_iteration:
	push esi ;save value
	mov esi, ebx
	xor esi, [ebp+local0]
	inc dword ptr [ebp+local0]

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

	xor eax, esi
	pop esi ; restore value

loop_epilogue:
	inc esi	
	loop hash_loop

exit:
	mov esp, ebp
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
	mov esp, ebp
	pop ebp
	ret 4
find_module_base endp

; *****************************
; arguments: dll base, function name hash
; *****************************
find_exported_func PROC
	push ebp
	mov ebp, esp
	sub esp, 0Ch
	
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
	mov [ebp+local1], eax ; save index
	
function_name_loop:
	mov [ebp+local2], ecx ; save loop counter
	mov edx, [ebp+local0] ; IMAGE_EXPORT_DIRECTORY
	mov ebx, [ebp+local1] ; index

	; get the i-th function name
	mov esi, [edx+020h] ; AddressOfNames RVA
	add esi, [ebp+arg0] ; AddressOfNames VA
	lea esi, [esi+TYPE DWORD*ebx] ; point to the current index	
	mov edi, [esi] ; function name RVA
	add edi, [ebp+arg0] ; function name VA
	
	; scan to find the NULL char
	xor eax, eax	
	mov esi, edi
	mov ecx, 512h
	repnz scasb

	; compute name length
	sub edi, esi
	dec edi

	; compute function name hash
	push edi
	push esi
	call hash_string

	; compare hash
	cmp eax, [ebp+arg1]
	je function_name_found

	; go to next name pointer
	inc dword ptr [ebp+local1]

	mov ecx, [ebp+local2]
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
	mov esp, ebp
	pop ebp
	ret 8
find_exported_func ENDP

; *****************************
; arguments: XOR key, dword
; *****************************
decode_dword PROC
	push ebp
	mov ebp, esp

	; get XOR key
	mov ecx, [ebp+arg0]

	; read dword to encode
	mov eax, [ebp+arg1]

	; step 1 XOR to get original second byte
	xor al, ah

	; step 2 ror
	ror eax, 10h

	; step 3 save value second byte
	xor cl, al
	shl cx, 8h

	; step 4 shlr
	mov ebx, eax
	shr ebx, 18h
	shrd ax, bx, 8h

	; step 5  hardcoded XOR
	xor ax, 0B9D3h
	
	; compose final value
	and eax, 0FFFFFFh
	shl ecx, 10h
	or eax, ecx

	mov esp, ebp
	pop ebp
	ret 8
decode_dword ENDP

; *****************************
; arguments: XOR key, dword
; *****************************
encode_dword PROC
	push ebp
	mov ebp, esp

	; get XOR key
	mov ecx, [ebp+arg0]

	; read dword to encode
	mov eax, [ebp+arg1]

	; step 1 hardcoded XOR
	xor ax, 0B9D3h

	; step 2 xor SP
	mov ebx, eax
	shr ebx, 018h
	xor bx, cx
	shl bx, 8h

	; step 3 SHLD
	mov si, ax ; save AX
	shr si, 8h ; save only first 2 bytes
	shld ax, bx, 8h

	; step 4 XOR missed bytes with third and four bytes
	mov edx, eax
	shr edx, 10h
	xor dx, si

	; compose final value	
	shl esi, 08h
	mov dh, 0h	
	or dx, si
	shl eax, 10h
	or ax, dx

	mov esp, ebp
	pop ebp
	ret 8
encode_dword ENDP

; *****************************
; arguments: func addr
; *****************************
relocate_code PROC
	push ebp
	mov ebp, esp
	sub esp, 10h
	
	; search the function footer
	mov edi, [ebp+arg0]
	mov ecx, 2018h ; dummy max value to search for
search_marker2:	
	cmp dword ptr [edi], marker2
	je search_marker1
	inc edi
	loopne search_marker2
	jne not_found	
	
	; verify if it is followed by marker1
search_marker1:
	add edi, TYPE DWORD
	cmp dword ptr [edi], marker1
	jne search_marker2	

	; compute function length
	lea edi, [edi-TYPE DWORD]
	mov eax, [ebp+arg0]
	sub edi, eax
	mov [ebp+local0], edi
	
	; find kernelbase addr
	push hash_kernelbase_dll
	call find_module_base
	mov [ebp+local1], eax

	; allocate memory
	push hash_VirtualAlloc
	push [ebp+local1]
	call find_exported_func	
	test eax, eax
	je not_found

	; call VirtualAlloc
	push PAGE_READWRITE
	push MEM_COMMIT
	push [ebp+local0]
	push 0h
	call eax
	test eax, eax
	je not_found
	mov [ebp+local3], eax

	; copy memory
	mov esi, [ebp+arg0]
	mov edi, [ebp+local3]
	mov ecx, [ebp+local0]	
	rep movsb

	; set exec flag
	push hash_VirtualProtect
	push [ebp+local1]
	call find_exported_func	
	test eax, eax
	je not_found
	
	; call VirtualProtect
	lea ebx, [ebp+local0]
	push ebx
	push PAGE_EXECUTE_READ
	push [ebp+local0]	
	push [ebp+local3]
	call eax
	test eax, eax
	je not_found
	jmp found

not_found:
	xor eax, eax
found:
	mov eax, [ebp+local3]
	mov esp, ebp
	pop ebp
	ret 04h
relocate_code ENDP

; *****************************
; arguments: func addr
; *****************************
free_relocated_code PROC
	push ebp
	mov ebp, esp
	sub esp, 8

	; find kernelbase addr
	push hash_kernelbase_dll
	call find_module_base
	mov [ebp+local0], eax
	test eax, eax
	je finish

	; free memory
	push hash_VirtualFree
	push [ebp+local0]
	call find_exported_func	
	test eax, eax
	je finish

	; call VirtualFree
	push MEM_RELEASE
	push 0h
	push [ebp+arg0]
	call eax

finish:
	mov esp, ebp
	pop ebp
	ret 04h
free_relocated_code ENDP