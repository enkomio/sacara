using ES.Sacara.Ir.Assembler;
using ES.SacaraVm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoadEncryptedAssembly
{
    internal class Encryption
    {
        private static readonly String _sacaraDll = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SacaraVm.dll");
        private static readonly Byte[] _deEncryptionCode = CreateScript();

        private static Byte[] CreateScript()
        {
            var assembler = new SacaraAssembler();
            var code = @"
proc main
    push buffer     
    push buffer_length
    push key        
    push key_length
    push 4          
    push de_encrypt 
    call
    halt
endp

/*
This method accept: 
1 - the length of the password
2 - a pointer to the password to use
3 - the lengh of the buffer
4 - a pointer to the buffer
*/
proc de_encrypt
    pop key_length
    pop key
    pop buffer_length
    pop buffer
    push 0
    pop buffer_index
    push 0
    pop key_index
    push 0
    pop buffer_char
    push 0
    pop key_char

encryption_loop:
    /* read the character from the buffer */
    push buffer_index
    push buffer
    add
    nread
    pop buffer_char

    /* read the character from the key buffer */
    push key_index
    push key
    add
    nread
    pop key_char

    /* do XOR and save the result on the stack */
    push key_char
    push buffer_char
    xor

    /* write back the result */
    push buffer_index
    push buffer
    add
    nwrite

    /* increase counter */
    push 1
    push key_index
    add
    pop key_index

    push 1
    push buffer_index
    add
    pop buffer_index

    /* check if I have to round the password index */    
    push key_length
    push key_index
    cmp
    push check_for_completation
    jumpifl

round_key:
    push 0
    pop key_index
    
check_for_completation: 
    push buffer_length 
    push buffer_index   
    cmp
    push encryption_loop
    jumpifl

    ret
endp
            ";
            var assembledCode = assembler.Assemble(code);
            return assembledCode.GetBuffer();
        }

        private static void DropSacaraVm()
        {
            if (!File.Exists(_sacaraDll))
                File.WriteAllBytes(_sacaraDll, Properties.Resources.SacaraVm);
        }

        public static void VmDecrypt(Byte[] buffer, String password)
        {
            DropSacaraVm();

            // prepare variables in order to be passed to the VM code
            var key = Encoding.Default.GetBytes(password);
            var passwordBuffer = Encoding.Default.GetBytes(password);
            var bufferPtr = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var keyPtr = GCHandle.Alloc(key, GCHandleType.Pinned);

            // execute the encryption routine
            using (var vm = new SacaraVm())
            {
                var index = 0;
                vm.LocalVarSet(index++, bufferPtr.AddrOfPinnedObject().ToInt32());
                vm.LocalVarSet(index++, buffer.Length);
                vm.LocalVarSet(index++, keyPtr.AddrOfPinnedObject().ToInt32());
                vm.LocalVarSet(index++, key.Length);
                vm.Run(_deEncryptionCode);
            }
        }

        public static void ManagedEncrypt(Byte[] buffer, String password)
        {
            var key = Encoding.Default.GetBytes(password);
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[i % key.Length]);
            }
        }
    }
}
