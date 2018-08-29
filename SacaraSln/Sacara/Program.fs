namespace ES.Sacara

open System
open ES.Sacara.Ir.Assembler

module Program =
    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main
            push local_var
            push get_code
            call
        some_label:
            pop result_var
            push some_label
            ret
        endp

        proc get_code
            push 0x42
            ret
        endp
        """
        let assembler = new SacaraAssembler()
        let code = assembler.Assemble(irCode)

        let vmBytes = code.GetBuffer()
        let vmListing = code.ToString()
        
        Console.WriteLine(vmListing)
        let bytesTring = 
            vmBytes 
            |> Seq.map(fun b -> b.ToString("X") + "h") 
            |> Seq.map(fun b -> 
                if 
                    b.StartsWith("a", StringComparison.OrdinalIgnoreCase) || b.StartsWith("b", StringComparison.OrdinalIgnoreCase) ||
                    b.StartsWith("c", StringComparison.OrdinalIgnoreCase) || b.StartsWith("d", StringComparison.OrdinalIgnoreCase) ||
                    b.StartsWith("e", StringComparison.OrdinalIgnoreCase) || b.StartsWith("f", StringComparison.OrdinalIgnoreCase)
                then "0" + b
                else b
            )
        Console.WriteLine("MASM: " + String.Join(",", bytesTring))
        0