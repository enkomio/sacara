namespace Examples.AssembleManagedInstructions

open System
open ES.Sacara.Ir.Assembler

module Program =
    (*
    This example use the builtin interface in order to write Sacara code in a F# program and in a more controlled environment.
    Even if you don't know F# it should be pretty easy to use it ;)
    The generated code can then be execute by the SacaraVM.
    *)

    [<EntryPoint>]
    let main argv =
        let assembler = new SacaraAssembler()
        let irInstruction = 
            [
                proc "main"         // define the main (entry point) function                
                inc "num1"          // increment by 1 the seconda value
                push "num1"         // push the incremented second value
                push "num2"         // push the first input value
                push 2              // push the number of arguments accepted by the invopked functions
                push "sum_numbers"  // push the function name to invoke
                call                // invoke the function
                pop "num1"          // read back the result
                halt                // exit
                endp

                proc "sum_numbers"
                add                 // add the two input numbers
                ret                 // return the result
                endp
            ] 
            |> List.map(fun f -> Action<Ctx>(f))
            |> List.toArray
            |> assembler.Assemble
        
        Console.WriteLine("-= Generated VM Code =-")
        Console.WriteLine(irInstruction)
        0