namespace ES.Sacara

open System
open ES.Sacara.Ir.Assembler

module Program =
    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main
            push label1
            jump
            nop
            nop
        label1:
            nop
            push get_code
            call
            halt
        endp

        proc get_code
            push 0x42
            push -10
            cmp
            ret
        endp
        """
        let assembler = new SacaraAssembler()
        assembler.Settings.RandomlyEncryptOpCode <- true
        let code = assembler.Assemble(irCode)

        let vmListing = code.ToString()        
        Console.WriteLine(vmListing)
        Console.WriteLine()

        Console.WriteLine("MASM:")
        let mutable index = 0
        code.Functions
        |> Seq.iter(fun irFunction ->
            irFunction.Body
            |> Seq.iter(fun opCode ->
                let bytesTring = 
                    opCode.Buffer
                    |> Seq.map(fun b -> b.ToString("X") + "h") 
                    |> Seq.map(fun b -> 
                        if 
                            b.StartsWith("a", StringComparison.OrdinalIgnoreCase) || b.StartsWith("b", StringComparison.OrdinalIgnoreCase) ||
                            b.StartsWith("c", StringComparison.OrdinalIgnoreCase) || b.StartsWith("d", StringComparison.OrdinalIgnoreCase) ||
                            b.StartsWith("e", StringComparison.OrdinalIgnoreCase) || b.StartsWith("f", StringComparison.OrdinalIgnoreCase)
                        then "0" + b
                        else b
                    )
                Console.WriteLine("code_{0} BYTE {1} ; {2}", index, String.Join(",", bytesTring).PadRight(30), opCode.ToString())
                index <- index + 1
            )
        )
        0