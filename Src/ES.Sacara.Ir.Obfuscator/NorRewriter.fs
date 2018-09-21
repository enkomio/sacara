namespace ES.Sacara.Ir.Obfuscator

open System
open ES.Sacara.Ir.Core

module NorRewriter =
    let private genOperand(name: String) = new Operand("TMP_" + name)

    let private push(operand: Operand, useMultipleOpcodeForSameInstruction: Boolean) =
        let push = new IrOpCode(IrOpCodes.Push, useMultipleOpcodeForSameInstruction)
        push.Operands.Add(operand)  
        push

    let private pop(operand: Operand, useMultipleOpcodeForSameInstruction: Boolean) =
        let pop = new IrOpCode(IrOpCodes.Pop, useMultipleOpcodeForSameInstruction)
        pop.Operands.Add(operand)
        pop
                
    let rewriteNot(useMultipleOpcodeForSameInstruction: Boolean) = [
        // NOT(a) = NOR(a, a)
        let op = genOperand("not")
        yield pop(op, useMultipleOpcodeForSameInstruction)
        yield push(op, useMultipleOpcodeForSameInstruction)
        yield push(op, useMultipleOpcodeForSameInstruction)
        yield new IrOpCode(IrOpCodes.Nor, useMultipleOpcodeForSameInstruction)
    ]

    let rewriteOr(useMultipleOpcodeForSameInstruction: Boolean) = [
        // OR(a, b) = NOT(NOR(a, b))
        yield new IrOpCode(IrOpCodes.Nor, useMultipleOpcodeForSameInstruction)
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) 
    ]

    let rewriteAnd(useMultipleOpcodeForSameInstruction: Boolean) = [
        // AND(a, b) = NOT(OR(NOT(a), NOT(b)))
        let op = genOperand("and")
        yield pop(op, useMultipleOpcodeForSameInstruction) // save a
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) // NOT(b)
        yield push(op, useMultipleOpcodeForSameInstruction) // write a
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) // NOT(a)
        yield! rewriteOr(useMultipleOpcodeForSameInstruction) // OR(NOT(a), NOT(b))
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) // NOT(OR(NOT(a), NOT(b)))
    ]

    let rewriteXor(useMultipleOpcodeForSameInstruction: Boolean) = [
        // XOR(a, b) = OR(AND(a, NOT(b)), AND(NOT(a), b)))
        let op1 = genOperand("xor1")
        let op2 = genOperand("xor2")

        yield pop(op1, useMultipleOpcodeForSameInstruction) // save a
        yield pop(op2, useMultipleOpcodeForSameInstruction) // save b        
        yield push(op2, useMultipleOpcodeForSameInstruction) // write b
        yield push(op1, useMultipleOpcodeForSameInstruction) // write a
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) // NOT(a)
        yield! rewriteAnd(useMultipleOpcodeForSameInstruction) // AND(NOT(a), b)
        yield push(op2, useMultipleOpcodeForSameInstruction) // write b
        yield! rewriteNot(useMultipleOpcodeForSameInstruction) // NOT(b)
        yield push(op1, useMultipleOpcodeForSameInstruction) // write a
        yield! rewriteAnd(useMultipleOpcodeForSameInstruction) // AND(a, NOT(b))
        yield! rewriteOr(useMultipleOpcodeForSameInstruction) // OR(AND(a, NOT(b)), AND(NOT(a), b)))
    ]