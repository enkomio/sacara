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
            call modify_code
            pop ip_var
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
        0