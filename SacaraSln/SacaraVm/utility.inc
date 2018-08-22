include const.inc

.data

kernel_32 DWORD 0

.code
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
	mov [ebp + local_0], eax	

find_func:
	push eax
	call compute_hash

	; check if the hash is the one that we are searching for. If so, traverse EAT to identify function by name

	pop eax

	mov eax, [eax]
	mov ecx, [ebp + local_0]
	cmp [ebp + local_0], eax
	jne find_func

	add esp, 4h

	mov ebp, esp
	pop ebp
	ret 4

invoke_kernel32 endp