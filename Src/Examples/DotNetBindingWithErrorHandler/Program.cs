using ES.Sacara.Ir.Assembler;
using ES.SacaraVm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetBindingWithErrorHandler
{
    public class Program
    {
        private static Boolean _errorRaised = false;

        private static void HandleError(UInt32 ip, UInt32 errorCode)
        {
            Console.WriteLine("Program faulted on the code following the one at offset {0}, error code: {1}", ip, ErrorCode.Parse(errorCode));
            _errorRaised = true;
        }

        public static void Main(string[] args)
        {
            var code = @"
proc main
    inc some_variable 
    byte 0x41, 0x41 /* this will generates an invalid instruction, raising an error */
    halt
endp
";
            var assembler = new SacaraAssembler();
            var vmCode = assembler.Assemble(code);
            Console.WriteLine(vmCode);

            // run the buggy code
            using (var vm = new SacaraVm())
            {
                vm.SetErrorHandler(HandleError);
                vm.Run(vmCode);
            }

            Debug.Assert(_errorRaised);
        }
    }
}
