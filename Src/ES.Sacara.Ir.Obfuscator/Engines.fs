namespace ES.Sacara.Ir.Obfuscator

open System
open System.Linq
open ES.Sacara.Ir.Core

[<AutoOpen>]
module Engines =
    let mutable private _random = new Random()

    let private shouldEncrypt(vmOpCode: VmOpCode) =
        let n = _random.Next(0,10)
        let opCodeToNotEncrypt = [VmByte; VmWord; VmDoubleWord]
        (opCodeToNotEncrypt |> List.contains vmOpCode.Type |> not) && ([3; 8] |> List.contains(n) |> not)

    let encryptVmOpCode(vmOpCode: VmOpCode) =
        if shouldEncrypt(vmOpCode) then
            let numericOpCode = BitConverter.ToUInt16(vmOpCode.VmOp, 0)
            let mutable encNumericOpCode = numericOpCode ^^^ uint16 0x5B ^^^ uint16(vmOpCode.Offset + vmOpCode.VmOp.Length)
            
            // clear flags
            encNumericOpCode <- encNumericOpCode &&& uint16 0xFFF

            // set encrypted flag (first bit)
            encNumericOpCode <- encNumericOpCode ||| uint16 0x8000
            
            let encOpCode = BitConverter.GetBytes(encNumericOpCode)
            Array.Copy(encOpCode, vmOpCode.Buffer, encOpCode.Length)
            vmOpCode.SyncOpAndOperandsWithBuffer()

    let encryptVmOperands(vmOpCode: VmOpCode) =
        if vmOpCode.Operands.Any() then
            // encrypt the operands
            let mutable operandLength = vmOpCode.Operands.[0].Length
            vmOpCode.Operands
            |> Seq.iteri(fun i operand ->
                let mutable encNumericOperandValue =
                    if operand.Length = 4 
                    then BitConverter.ToUInt32(operand, 0)
                    else  BitConverter.ToUInt16(operand, 0) |> uint32

                let hardCodedKey =
                    if operand.Length = 4 
                    then uint32 0x547431C0
                    else uint32 0xCA50

                let encKeyItem = (vmOpCode.Offset + vmOpCode.VmOp.Length + (operandLength * i) + operand.Length) |> uint32
                let encKey =
                    encKeyItem |||
                    (encKeyItem <<< 8) |||
                    (encKeyItem <<< 16) |||
                    (encKeyItem <<< 24)

                encNumericOperandValue <- encNumericOperandValue ^^^ hardCodedKey ^^^ encKey

                let encOperand =
                    if operand.Length = 4 
                    then BitConverter.GetBytes(encNumericOperandValue)
                    else BitConverter.GetBytes(uint16 encNumericOperandValue)

                let operandOffset = vmOpCode.VmOp.Length + (operandLength * i)
                Array.Copy(encOperand, 0, vmOpCode.Buffer, operandOffset, encOperand.Length)
            )

            // set encrypted flag (second bit)
            let numericOpCode = BitConverter.ToUInt16(vmOpCode.VmOp, 0)
            let encNumericOpCode = numericOpCode ||| uint16 0x4000
            let encOpCode = BitConverter.GetBytes(encNumericOpCode)
            Array.Copy(encOpCode, vmOpCode.Buffer, encOpCode.Length)

            // sync fields with buffer value
            vmOpCode.SyncOpAndOperandsWithBuffer()

    let private applyLabel(irCode: IrOpCode) (rewrittenOpCodes: IrOpCode list) =
        rewrittenOpCodes.[0].Label <- irCode.Label
        rewrittenOpCodes
    
    let reWriteInstructionWithNorOperator(body: IrOpCode seq, useMultipleOpcodeForSameInstruction: Boolean) =        
        body
        |> Seq.map(fun irCode ->
            let setLabel = applyLabel irCode
            match irCode.Type with
            | Not -> NorRewriter.rewriteNot(useMultipleOpcodeForSameInstruction) |> setLabel
            | And -> NorRewriter.rewriteAnd(useMultipleOpcodeForSameInstruction) |> setLabel
            | Or -> NorRewriter.rewriteOr(useMultipleOpcodeForSameInstruction) |> setLabel
            | Xor -> NorRewriter.rewriteXor(useMultipleOpcodeForSameInstruction) |> setLabel
            | _ -> [irCode]
        )
        |> Seq.concat