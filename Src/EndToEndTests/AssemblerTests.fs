namespace Sacara.EndToEndTests

open System
open ES.Sacara.Ir.Assembler
open ES.Sacara.Ir.Core

module AssemblerTests =

    let private assembleCode(code: String) =
        let assembler = new SacaraAssembler()
        let vmCode = assembler.Assemble(code)
        vmCode.Functions.[0].Body

    let private assembleSingleInstruction(instructionText: String, irInstruction: IrInstruction, vmInstruction: VmInstruction) =
        String.Format("""
            proc main
                {0}
            endp
        """, instructionText)
        |> assembleCode
        |> fun instructions ->
            assert(instructions.Length = 1)
            assert(instructions.[0].IrOp.Type = irInstruction)
            assert(instructions.[0].Type = vmInstruction)

    let ``Assemble halt - default settings``() =
        assembleSingleInstruction("halt", IrInstruction.Halt, VmInstruction.VmHalt)

