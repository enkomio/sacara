namespace ES.Sacara

open System
open ES.Sacara.Ir.Assembler

module Program =
    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main  
            push start_code
            jump
            BYTE "Ciao a tutti!!",0
            WORD 0x1234,0x5678
            DWORD 0x12345678

        start_code:
            push 44
            push start            
            jump        
        start:
            getip
            pop local0
            getsp
            pop local1
            halt
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