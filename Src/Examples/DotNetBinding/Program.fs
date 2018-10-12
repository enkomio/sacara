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
                pop "result"
                halt
                endp
            ] |> assembler.Assemble
        
        irInstruction.GetBuffer()

    [<EntryPoint>]
    let main argv =
        let sacaraCode = generateBytecode()

        let vm = new SacaraVm()
        vm.Init(sacaraCode)
        vm.Run()
        let result = vm.LocalVarGet(0)
        Console.WriteLine(String.Format("Code execution result: {0}", result))
        0