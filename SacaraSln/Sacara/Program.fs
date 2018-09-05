namespace ES.Sacara

open System
open ES.Sacara.Ir.Assembler

module Program =
    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main  
            push start_code
            call
            halt
        data:
            BYTE "Ciao a tutti!!",0
            WORD 0x1234,0x5678
            DWORD 0x12345678
        endp

        proc start_code
            push data
            pop data_var
        data2:
            getip
            pop local0
            getsp
            pop local1
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
                Console.WriteLine("code_{0} BYTE {1} ; {2}", index.ToString("0000"), String.Join(",", bytesTring).PadRight(40), opCode.ToString())
                index <- index + 1
            )
        )
        0