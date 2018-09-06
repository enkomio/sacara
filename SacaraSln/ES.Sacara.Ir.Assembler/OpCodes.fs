namespace ES.Sacara.Ir.Assembler

open System
open System.Collections.Generic
open System.Text

type SymbolType =
    | LocalVar
    | Label

type Symbol = {
    Name: String
    Offset: Int32
    Type: SymbolType
}

type Operand(value: Object) =
    let encodeRawData(bytes: Int32 list, vmOpCodeType: VmOpCodes) =
        match vmOpCodeType with
        | VmOpCodes.VmByte ->
            bytes          
            |> Seq.map(fun b -> byte b)
            |> Seq.toArray

        | VmOpCodes.VmWord ->
            bytes            
            |> Seq.map(fun b -> BitConverter.GetBytes(uint16 b))
            |> Seq.concat
            |> Seq.toArray

        | VmOpCodes.VmDoubleWord ->
            bytes            
            |> Seq.map(BitConverter.GetBytes)
            |> Seq.concat
            |> Seq.toArray

        | _ -> failwith "Wrong Raw data opcode"

    member val Value = value with get, set

    member this.Encode(symbolTable: SymbolTable, offset: Int32, vmOpCodeType: VmOpCodes) =
        match this.Value with
        | :? list<Int32> -> encodeRawData(this.Value :?> list<Int32>, vmOpCodeType)
        | :? Int32 -> BitConverter.GetBytes(this.Value :?> Int32)
        | :? String ->
            let identifier = this.Value.ToString()
            match vmOpCodeType with
            | VmPushVariable ->
                if symbolTable.IsLabel(identifier) then
                    symbolTable.GetLabel(identifier, offset).Offset
                    |> uint32
                    |> BitConverter.GetBytes
                else
                    symbolTable.GetVariable(identifier).Offset
                    |> uint16
                    |> BitConverter.GetBytes
            | VmPop ->
                symbolTable.GetVariable(identifier).Offset
                |> uint16
                |> BitConverter.GetBytes
            | _ ->
                // by default use 32 bit operand
                symbolTable.GetLabel(identifier, offset).Offset
                |> uint32
                |> BitConverter.GetBytes

        | _ -> failwith "Unrecognized symbol type"        

    override this.ToString() =
        this.Value.ToString()

and VmOpCode = {
    IrOp: IrOpCode
    VmOp: Byte array
    Type: VmOpCodes
    Operands: List<Byte array>
    Buffer: Byte array
    Offset: Int32
} with

    member this.IsInOffset(offset: Int32) =
        offset >= this.Offset && this.Offset + this.Buffer.Length > offset

    override this.ToString() =
        let bytes = BitConverter.ToString(this.Buffer).Replace("-", String.Empty).PadLeft(12)
        let offset = this.Offset.ToString("X").PadLeft(8, '0')

        let operands =
            match this.Type with
            | VmOpCodes.VmByte ->
                let sb = new StringBuilder()
                let mutable inString = false
                this.Operands.[0]
                |> Seq.iter(fun b ->
                    if Char.IsControl(char b) then 
                        if inString then
                            sb.Append("\"") |> ignore
                            inString <- false
                        sb.AppendFormat(",{0}", b.ToString("X")) |> ignore
                    else 
                        if not inString then 
                            sb.Append("\"") |> ignore
                            inString <- true
                        sb.Append(char b) |> ignore
                )
                let resultString = sb.ToString()
                if resultString.StartsWith(",")
                then resultString.Substring(1)
                else resultString
            | VmOpCodes.VmWord ->
                this.Operands.[0]
                |> Seq.chunkBySize 2
                |> Seq.map(fun b -> "0x" + BitConverter.ToUInt16(b, 0).ToString("X"))
                |> fun s -> String.Join(", ", s)
            | VmOpCodes.VmDoubleWord ->
                this.Operands.[0]
                |> Seq.chunkBySize 4
                |> Seq.map(fun b -> "0x" + BitConverter.ToUInt32(b, 0).ToString("X"))
                |> fun s -> String.Join(", ", s)
            | _ ->
                this.Operands
                |> Seq.map(fun bytes ->
                    if bytes.Length = 2 
                    then BitConverter.ToInt16(bytes, 0).ToString("X")
                    else BitConverter.ToInt32(bytes, 0).ToString("X")
                )
                |> Seq.map(fun num -> String.Format("0x{0}", num))
                |> fun nums -> String.Join(", ", nums)

        String.Format("/* {0} */ loc_{1}: {2} {3}", bytes, offset, this.Type, operands)
    
    static member Assemble(vmType: VmOpCodes, vmOp: Byte array, operands: List<Byte array>, offset: Int32, irOp: IrOpCode) =
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
            Type = vmType
            Operands = operands
            Buffer = buffer
            Offset = offset
        }
                
    member this.SyncOpAndOperandsWithBuffer() =
        // sync op
        Array.Copy(this.Buffer, 0, this.VmOp, 0, this.VmOp.Length)

        // sync operands
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
    
    let getVmOpCode(opCode: VmOpCodes, settings: AssemblerSettings) =
        let opCodes = Instructions.bytes.[opCode]
        (chooseRepresentation(opCodes, settings), opCode)
    
    let getNotPureStackBasedVmOpCode(operands: Operand seq, symbolTable: SymbolTable, indexes: VmOpCodes list, settings: AssemblerSettings) =                
        let firstOperand = operands |> Seq.head
        let vmOpCode = 
            match firstOperand.Value with
            | :? Int32 -> indexes.[0]
            | :? String -> 
                // check the identifier, if is a label (or a functiona name) then we had to emit an immediate
                let identifier = firstOperand.Value.ToString()
                if symbolTable.IsLabel(identifier)
                then indexes.[0]                
                else indexes.[1]
            | _ -> failwith "Invalid operand type"

        let opCodes = Instructions.bytes.[vmOpCode]
        (chooseRepresentation(opCodes, settings), vmOpCode)

    let getMacroOpCodeBytes(vmOpCode: VmOpCodes) =
        // macro doesn't have any bytes as opCode, I have just to encode the operands
        (Array.empty<Byte>, vmOpCode)
        
    member val Type = opType with get
    member val Operands = new List<Operand>() with get
    member val Label: String option = None with get, set

    member this.Assemble(ip: Int32, symbolTable: SymbolTable, settings: AssemblerSettings) =
        let operands = new List<Byte array>()
        
        // encode the operation
        let (opBytes, vmOpCode) =
            match this.Type with
            | Ret -> getVmOpCode(VmRet, settings)
            | Nop -> getVmOpCode(VmNop, settings)
            | Add -> getVmOpCode(VmAdd, settings)
            | Push -> getNotPureStackBasedVmOpCode(this.Operands, symbolTable, [VmPushImmediate; VmPushVariable], settings)
            | Pop -> getVmOpCode(VmPop, settings)
            | Call -> getVmOpCode(VmCall, settings)
            | NativeCall -> getVmOpCode(VmNativeCall, settings)
            | Read -> getVmOpCode(VmRead, settings)
            | NativeRead -> getVmOpCode(VmNativeRead, settings)
            | Write -> getVmOpCode(VmWrite, settings)
            | NativeWrite -> getVmOpCode(VmNativeWrite, settings)
            | GetIp -> getVmOpCode(VmGetIp, settings)
            | Jump -> getVmOpCode(VmJump, settings)
            | JumpIfLess -> getVmOpCode(VmJumpIfLess, settings)
            | JumpIfLessEquals -> getVmOpCode(VmJumpIfLessEquals, settings)
            | JumpIfGreat -> getVmOpCode(VmJumpIfGreat, settings)
            | JumpIfGreatEquals -> getVmOpCode(VmJumpIfGreatEquals, settings)
            | Alloca -> getVmOpCode(VmAlloca, settings)
            | Halt -> getVmOpCode(VmHalt, settings)
            | Cmp -> getVmOpCode(VmCmp, settings)
            | GetSp -> getVmOpCode(VmGetSp, settings)
            | StackWrite -> getVmOpCode(VmStackWrite, settings)
            | StackRead -> getVmOpCode(VmStackRead, settings)
            | Sub -> getVmOpCode(VmSub, settings)
            | Mul -> getVmOpCode(VmMul, settings)
            | Div -> getVmOpCode(VmDiv, settings)
            | And -> getVmOpCode(VmAnd, settings)
            | Or -> getVmOpCode(VmOr, settings)
            | Not -> getVmOpCode(VmNot, settings)
            | Xor -> getVmOpCode(VmXor, settings)
            | ShiftLeft -> getVmOpCode(VmShiftLeft, settings)
            | ShiftRight -> getVmOpCode(VmShiftRight, settings)
            | Byte -> getMacroOpCodeBytes(VmByte)
            | Word -> getMacroOpCodeBytes(VmWord)
            | DoubleWord -> getMacroOpCodeBytes(VmDoubleWord)           
            
        // encode the operands
        this.Operands
        |> Seq.iter(fun operand ->
            let operandBytes = operand.Encode(symbolTable, ip + opBytes.Length, vmOpCode)
            operands.Add(operandBytes)
        )

        // return the VM opcode
        VmOpCode.Assemble(vmOpCode, opBytes, operands, ip, this)

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
    let _labelNames = new List<String>()
    let _placeholders = new List<String * Int32>()

    member this.StartFunction() =
        _variables.Clear()

    member this.AddLocalVariable(name: String) =
        if _labelNames.Contains(name)
        then failwith("Unable to add the local variable '" + name + "', since already exists a function/label with the same name")
        _variables.[name] <- {Name=name; Offset=_variables.Count; Type=LocalVar}

    member this.AddLabel(name: String, offset: Int32) =
        if _labels.ContainsKey(name)
        then failwith (String.Format("Each label and function name must be unique. The label '{0}' was already defined, please choose another name", name))
        _labels.Add(name, {Name=name; Offset=offset; Type=Label})

    member this.AddLabelName(funcName: String) =
        if _labelNames.Contains(funcName)
        then failwith (String.Format("Each label and function name must be unique. The function '{0}' was already defined, please choose another name", funcName))        
        _labelNames.Add(funcName)

    member this.IsLabel(funcName: String) =
        _labelNames.Contains(funcName)

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
                    let bytes = BitConverter.GetBytes(_labels.[name].Offset)

                    // fix the buffer
                    Array.Copy(bytes, 0, vmOpCode.Buffer, relativeOffsetToFix, bytes.Length)
                    vmOpCode.SyncOpAndOperandsWithBuffer()
            )
        )
        
and IrFunction (name: String) =
    member val Name = name with get
    member val Body = new List<IrOpCode>() with get
    member val IsEntryPoint = false with get, set

and VmFunction = {
    Body: VmOpCode list
}