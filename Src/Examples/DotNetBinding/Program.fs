namespace Examples.DotNetBinding

open System
open ES.SacaraVm
open ES.Sacara.Ir.Assembler

module Program =
    (*
    This example use the SacaraVm .NET binding and the Sacara assembler in order to execute the code
    *)

    let generateBytecode() =
        let assembler = new SacaraAssembler()
        let irInstruction = 
            [
                proc "main"
                push 12
                push 44
                add
                pop "result" // save the result in a local var to access it
                halt
                endp
            ] |> assembler.Assemble
        
        irInstruction.GetBuffer()

    let runCode(code: Byte array) =
        use vm = new SacaraVm()        
        vm.Run(code)
        vm.LocalVarGet(0)

    [<EntryPoint>]
    let main argv =
        let code = generateBytecode()
        let result = runCode(code)        
        Console.WriteLine(String.Format("Code execution result: {0}", result))
        0