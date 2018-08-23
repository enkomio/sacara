; arguments: start_memory, size, marker
find_marker PROC
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
find_marker ENDP


;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
compute_hash proc
	push ebp
	mov ebp, esp

	mov eax, [ebp+arg0]
	mov edi, [eax+20h] ; UNICODE module name
	mov eax, [eax+10h] ; DllBase
	test eax, eax
	je exit

	add edi, 2 ; skip the drive name

	xor eax, eax
	mov esi, edi
	repne scasw ; find the end of the string

	xor eax, eax

hash_loop: 
	xor eax, esi
	rol eax, 8
	lea esi, [esi + 4]
	cmp esi, edi
	jl hash_loop

exit:
	mov ebp, esp
	pop ebp
	ret

compute_hash endp

invoke_kernel32 proc
	push ebp
	mov ebp, esp

	sub esp, 4h

	assume fs:nothing
	mov eax, fs:[30h]  ; PEB
	assume fs:error
	
	mov eax, [eax+0ch] ; Ldr
	mov eax, [eax+14h] ; InMemoryOrderModuleList
	mov [ebp + local0], eax	

find_func:
	push eax
	call compute_hash

	; check if the hash is the one that we are searching for. If so, traverse EAT to identify function by name

	pop eax

	mov eax, [eax]
	mov ecx, [ebp + local0]
	cmp [ebp + local0], eax
	jne find_func

	add esp, 4h

	mov ebp, esp
	pop ebp
	ret 4

invoke_kernel32 endp