namespace ES.Sacara.Ir.Assembler

open System
open System.Collections.Generic

type SymbolType =
    | LocalVar
    | Label

type Symbol = {
    Name: String
    Offset: Int32
    Type: SymbolType
}

type Operand(value: Object) =
    member val Value = value with get, set

    member this.Encode(symbolTable: SymbolTable, offset: Int32, opCodeType: IrOpCodes) =
        match this.Value with
        | :? Int32 -> BitConverter.GetBytes(this.Value :?> Int32)
        | :? String ->
            if opCodeType.AcceptLabel() then 
                symbolTable.GetLabel(this.Value.ToString(), offset).Offset
                |> uint32
                |> BitConverter.GetBytes
            else 
                symbolTable.GetVariable(this.Value.ToString()).Offset
                |> uint16
                |> BitConverter.GetBytes
        | _ -> failwith "Unrecognized symbol type"
        // to make the result little endian
        |> Array.rev

    override this.ToString() =
        this.Value.ToString()

and VmOpCode = {
    IrOp: IrOpCode
    VmOp: Byte array
    Operands: List<Byte array>
    Buffer: Byte array
    Offset: Int32
} with

    member this.IsInOffset(offset: Int32) =
        offset >= this.Offset && this.Offset + this.Buffer.Length > offset
    
    static member Assemble(vmOp: Byte array, operands: List<Byte array>, offset: Int32, irOp: IrOpCode) =
        let totalSize = vmOp.Length + (operands |> Seq.sumBy(fun op -> op.Length))

        let buffer = Array.zeroCreate<Byte>(totalSize)
        
        // write operation
        Array.Copy(vmOp, buffer, vmOp.Length)
        let mutable currOffset = vmOp.Length

        // write operands
        operands
        |> Seq.iter(fun bytes ->
            Array.Copy(bytes, 0, buffer, currOffset, bytes.Length)
            currOffset <- currOffset + bytes.Length
        )

        {
            IrOp = irOp
            VmOp = vmOp
            Operands = operands
            Buffer = buffer
            Offset = offset
        }

    member this.FixOperands() =
        let opCodeSize = this.VmOp.Length
        this.Operands
        |> Seq.toList
        |> List.iteri(fun i operand ->
            let startOffset = opCodeSize + i * operand.Length
            let endOffset = startOffset + operand.Length
            this.Operands.[i] <- this.Buffer.[startOffset..endOffset-1]
        )


and IrOpCode(opType: IrOpCodes) =
    let rnd = new Random()

    let chooseRepresentation(opCodes: Int32 list, settings: AssemblerSettings) =
        if settings.UseMultipleOpcodeForSameInstruction
            then opCodes.[rnd.Next(opCodes.Length)]
            else opCodes.[0]
        |> uint16
        |> BitConverter.GetBytes        
        // to make the result little endian
        |> Array.rev
    
    let getSimpleOpCode(opCode: VmOpCodes, settings: AssemblerSettings) =
        let opCodes = Instructions.bytes.[opCode]
        chooseRepresentation(opCodes, settings)
    
    let resolveOpCodeForImmOrVariable(operands: Operand seq, indexes: VmOpCodes list, settings: AssemblerSettings) =                
        let firstOperand = operands |> Seq.head
        let opCodes =
            match firstOperand.Value with
            | :? Int32 -> Instructions.bytes.[indexes.[0]]
            | :? String -> Instructions.bytes.[indexes.[1]]
            | _ -> failwith "Invalid operand type"
        chooseRepresentation(opCodes, settings)

    member val Type = opType with get
    member val Operands = new List<Operand>() with get
    member val Label: String option = None with get, set

    member this.Assemble(ip: Int32, symbolTable: SymbolTable, settings: AssemblerSettings) =
        let operands = new List<Byte array>()
        
        // encode the operation
        let opBytes =
            match this.Type with
            | Ret -> getSimpleOpCode(VmRet, settings)
            | Nop -> getSimpleOpCode(VmNop, settings)
            | Add -> getSimpleOpCode(VmAdd, settings)
            | Push -> resolveOpCodeForImmOrVariable(this.Operands, [VmPushImmediate; VmPushVariable], settings)
            | Pop -> getSimpleOpCode(VmPop, settings)
            | Call -> resolveOpCodeForImmOrVariable(this.Operands, [VmCallImmediate; VmCallVariable], settings)
            | NativeCall -> resolveOpCodeForImmOrVariable(this.Operands, [VmNativeCallImmediate; VmNativeCallVariable], settings)
            | Read -> resolveOpCodeForImmOrVariable(this.Operands, [VmReadImmediate; VmReadVariable], settings)
            | NativeRead -> resolveOpCodeForImmOrVariable(this.Operands, [VmNativeReadImmediate; VmNativeReadVariable], settings)
            | Write -> resolveOpCodeForImmOrVariable(this.Operands, [VmWriteImmediate; VmWriteVariable], settings)
            | NativeWrite -> resolveOpCodeForImmOrVariable(this.Operands, [VmNativeWriteImmediate; VmNativeWriteVariable], settings)
            | GetIp -> getSimpleOpCode(VmGetIp, settings)
            | Jump -> resolveOpCodeForImmOrVariable(this.Operands, [VmJumpImmediate; VmJumpVariable], settings)
            | JumpIfLess -> resolveOpCodeForImmOrVariable(this.Operands, [VmJumpIfLessImmediate; VmJumpIfLessVariable], settings)
            | JumpIfLessEquals -> resolveOpCodeForImmOrVariable(this.Operands, [VmJumpIfLessEqualsImmediate; VmJumpIfLessEqualsVariable], settings)
            | JumpIfGreat -> resolveOpCodeForImmOrVariable(this.Operands, [VmJumpIfGreatImmediate; VmJumpIfGreatVariable], settings)
            | JumpIfGreatEquals -> resolveOpCodeForImmOrVariable(this.Operands, [VmJumpIfGreatEqualsImmediate; VmJumpIfGreatEqualsVariable], settings)
            | Alloca -> getSimpleOpCode(VmAlloca, settings)
            | Byte -> getSimpleOpCode(VmByte, settings)
            | Word -> getSimpleOpCode(VmWord, settings)
            | DoubleWord -> getSimpleOpCode(VmDoubleWord, settings)
            | Halt -> getSimpleOpCode(VmHalt, settings)
            | Cmp -> getSimpleOpCode(VmCmp, settings)
            // to make the result little endian
            |> Array.rev

        // encode the operands
        this.Operands
        |> Seq.iter(fun operand ->
            operands.Add(operand.Encode(symbolTable, ip + opBytes.Length, this.Type))
        )

        // return the VM opcode
        VmOpCode.Assemble(opBytes, operands, ip, this)

    override this.ToString() =
        let ops = String.Join(", ", this.Operands)
        let label = 
            if this.Label.IsSome
            then this.Label.Value + ": "
            else String.Empty
        String.Format("{0}{1} {2}", label, this.Type, ops)

and SymbolTable() =
    let _variables = new Dictionary<String, Symbol>()    
    let _labels = new Dictionary<String, Symbol>()
    let _placeholders = new List<String * Int32>()

    member this.StartFunction() =
        _variables.Clear()

    member this.AddLocalVariable(name: String) =
        _variables.[name] <- {Name=name; Offset=_variables.Count+1; Type=LocalVar}

    member this.AddLabel(name: String, offset: Int32) =
        _labels.[name] <- {Name=name; Offset=offset; Type=Label}

    member this.GetVariable(name: String) : Symbol =
        if _variables.ContainsKey(name) then 
            _variables.[name]
        else
            this.AddLocalVariable(name)
            _variables.[name]

    member this.GetLabel(name: String, ip: Int32) : Symbol =
        if _labels.ContainsKey(name) then
            _labels.[name]
        else
            // create a placeholder, the offset is the IP of the placeholder that will be valorized   
            let placeholder = {Name=name; Offset=0xBAADF00D; Type=Label} 
            _placeholders.Add((name, ip))            
            placeholder

    member this.FixPlaceholders(vmFunctions: VmFunction list) =
        // replace the values with the correct VM IP
        _placeholders
        |> Seq.iter(fun (name, offset) ->
            vmFunctions
            |> List.map(fun vmFunction -> vmFunction.Body)
            |> List.concat
            |> List.iter(fun vmOpCode ->
                if vmOpCode.IsInOffset(offset) then
                    // I found the VM opcode that need to be fixed
                    let relativeOffsetToFix = offset - vmOpCode.Offset
                    let bytes = 
                        BitConverter.GetBytes(_labels.[name].Offset)
                        // to make the result little endian
                        |> Array.rev

                    // fix the buffer
                    Array.Copy(bytes, 0, vmOpCode.Buffer, relativeOffsetToFix, bytes.Length)
                    vmOpCode.FixOperands()
            )
        )
        
and IrFunction (name: String) =
    member val Name = name with get
    member val Body = new List<IrOpCode>() with get

and VmFunction = {
    Body: VmOpCode list
}