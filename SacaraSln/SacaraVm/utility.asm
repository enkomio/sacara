; *****************************
; arguments: start_memory, size, marker
; *****************************
find_handler PROC
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
search_opcode_loop:	
	add eax, 4
	cmp dword ptr [eax], esi
	loopne search_opcode_loop

	; jump if this is not the header that we are searching for	
	pop ecx
	jne search_header_loop

	; function found, save the read address in EAX
	; EDI contains the address of marker2
	lea eax, [edi + 4*edx + 8]
	jmp found

not_found:
	xor eax, eax
found:
	mov ebp, esp
	pop ebp
	ret 0Ch
find_handler ENDP


; *****************************
; arguments: modulo hash
; *****************************
find_module PROC
	push ebp
	mov ebp, esp

	sub esp, 4

	; read PEB
	assume fs:nothing
	mov eax, fs:[30h]
	assume fs:error

	; read module list
	mov eax, [eax+0ch] ; Ldr
	mov eax, [eax+14h] ; InMemoryOrderModuleList
	mov [ebp+local0], eax ; save location

	add esp, 4

	mov ebp, esp
	pop ebp
	ret 4h
find_module ENDP


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
	dec ecx
	test ecx, ecx
	jnz hash_loop

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
	; see: https://stackoverflow.com/questions/31512952/link-structures-ldr-peb
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