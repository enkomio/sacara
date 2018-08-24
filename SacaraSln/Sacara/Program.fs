namespace ES.Sacara

open System
open ES.Sacara.Ir.Assembler

module Program =
    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main
            jump label1
            label1: push 52
            /* this is a 
            multi
            line
            comment */ 
            push 62            
            add    
            pop local_var
            jump label2
            byte 0xff
            label2:
            push modify_code
            call
            pop ip_var
            halt
            ret
        endp

        proc modify_code
            getip
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