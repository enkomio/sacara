namespace Sacara.EndToEndTests

open System
open ES.Sacara.Ir.Assembler
open ES.Sacara.Ir.Core

module internal Utility =
    let assembleCode(code: String) =
        let assembler = new SacaraAssembler()
        let vmCode = assembler.Assemble(code)
        vmCode.Functions.[0].Body

    let assembleInstructionWithArg(irInstruction: IrInstruction, vmInstruction: VmInstruction, arg: String) =
        Console.WriteLine("Test instruction assembly: {0} {1}, VM: {2}", irInstruction, arg, vmInstruction)
        String.Format("""
            proc main
                {0} {1}
            endp
        """, irInstruction, arg)
        |> assembleCode
        |> fun instructions ->            
            // if it need an argument the instructions numbers may vary according 
            // if it is a variable or an immediate
            assert(instructions.Length = 1 || instructions.Length = 3)
            let lastInstruction = instructions |> List.last
            assert(lastInstruction.IrOp.Type = irInstruction)
            assert(lastInstruction.Type = vmInstruction)

    let assembleInstruction(irInstruction: IrInstruction, vmInstruction: VmInstruction) =
        assembleInstructionWithArg(irInstruction, vmInstruction, String.Empty)

