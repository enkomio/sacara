namespace ES.Sacara.Ir.Assembler

open System
open System.Collections.Generic
open ES.Sacara.Ir.Parser
open ES.Sacara.Ir.Parser.IrAst

type IrAssemblyCode = {
    Functions: VmFunction list
} with
    member this.GetBuffer() =
        this.Functions
        |> List.map(fun vmFunction -> vmFunction.Body)
        |> List.concat
        |> List.map(fun vmOpCode -> vmOpCode.Buffer)
        |> Array.concat

    override this.ToString() =
        this.Functions
        |> List.map(fun vmFunction -> vmFunction.Body)
        |> List.concat
        |> List.map(fun vmOpCode ->
            let bytes = BitConverter.ToString(vmOpCode.Buffer).Replace("-", String.Empty).PadLeft(12)
            let offset = vmOpCode.Offset.ToString("X").PadLeft(8, '0')
            let operands = BitConverter.ToString(vmOpCode.Operands |> Seq.concat |> Seq.toArray).Replace("-", String.Empty)
            String.Format("/* {0} */ {1}: {2} {3}", bytes, offset, vmOpCode.IrOp.Type, operands)
        )
        |> fun l -> String.Join(Environment.NewLine, l)

type SacaraAssembler() =
    let mutable _currentFunction = new IrFunction(String.Empty)
    let mutable _functions = new List<IrFunction>()
    let mutable _currentLabel: String option = None
    let mutable _currentIp = 0

    let addOperand(opCode: IrOpCode) =
        if _currentLabel.IsSome then
            opCode.Label <- _currentLabel
            _currentLabel <- None
        _currentFunction.Body.Add(opCode)

    let rec parseExpression(expression: Expression) =
        match expression with
        | Number num -> new Operand(num)
        | Identifier identifier -> new Operand(identifier)

    let rec parseStatement(statement: Statement) =
        match statement with
        | Procedure procType ->            
            _currentFunction <- new IrFunction(procType.Name)
            _functions.Add(_currentFunction)
            procType.Body |> List.iter(parseStatement)
        | Push pushType ->
            let push = new IrOpCode(IrOpCodes.Push)
            push.Operands.Add(parseExpression(pushType.Operand))
            addOperand(push)
        | Pop popType ->
            let pop = new IrOpCode(IrOpCodes.Pop)
            pop.Operands.Add(new Operand(popType.Identifier))
            addOperand(pop)
        | Label labelType -> 
            _currentLabel <- Some labelType.Name
            parseStatement(labelType.Statement)
        | Call callType -> 
            let call = new IrOpCode(if callType.Native then IrOpCodes.NativeCall else IrOpCodes.Call)
            call.Operands.Add(parseExpression(callType.Operand))
            addOperand(call)
        | Read readType ->
            let read = new IrOpCode(if readType.Native then IrOpCodes.NativeRead else IrOpCodes.Read)
            read.Operands.Add(parseExpression(readType.Operand))
            addOperand(read)
        | Write writeType ->
            let write = new IrOpCode(if writeType.Native then IrOpCodes.NativeWrite else IrOpCodes.Write)
            write.Operands.Add(parseExpression(writeType.DestOperand))
            write.Operands.Add(new Operand(writeType.SourceOperand))
            addOperand(write)
        | Nop -> 
            addOperand(new IrOpCode(IrOpCodes.Nop))
        | GetIp ->
            addOperand(new IrOpCode(IrOpCodes.GetIp))
        | Add ->
            addOperand(new IrOpCode(IrOpCodes.Add))
        | Ret ->
            addOperand(new IrOpCode(IrOpCodes.Ret))
        | JumpIf jumpIfType -> 
            let opCode =
                match (jumpIfType.JumpIfEquals, jumpIfType.JumpIfLess) with
                | (true, true) -> IrOpCodes.JumpIfLessEquals
                | (true, false) -> IrOpCodes.JumpIfGreatEquals
                | (false, true) -> IrOpCodes.JumpIfLess
                | (false, false) -> IrOpCodes.JumpIfGreat

            let jumpIf = new IrOpCode(opCode)
            jumpIf.Operands.Add(parseExpression(jumpIfType.Destination))
            addOperand(jumpIf)
        | Jump jumpType ->
            let jump = new IrOpCode(IrOpCodes.Jump)
            jump.Operands.Add(parseExpression(jumpType.Destination))
            addOperand(jump)
        | Empty -> ()
        | Alloca stackItems ->
            let alloca = new IrOpCode(IrOpCodes.Alloca)
            alloca.Operands.Add(new Operand(stackItems))
            addOperand(alloca)
        | Byte b ->
            let byte = new IrOpCode(IrOpCodes.Byte)
            byte.Operands.Add(new Operand(b))
            addOperand(byte)
        | Word w ->
            let word = new IrOpCode(IrOpCodes.Word)
            word.Operands.Add(new Operand(w))
            addOperand(word)
        | DoubleWord dw ->
            let dword = new IrOpCode(IrOpCodes.DoubleWord)
            dword.Operands.Add(new Operand(dw))
            addOperand(dword)
        | Halt ->
            addOperand(new IrOpCode(IrOpCodes.Halt))
        | Cmp ->
            addOperand(new IrOpCode(IrOpCodes.Cmp))

    let parseAst(ast: Program) =
        match ast with
        | Program sl -> sl |> List.iter(parseStatement)

    let assemblyIrOpCode(symbolTable: SymbolTable, settings: AssemblerSettings) (opCode: IrOpCode) =
        if opCode.Label.IsSome then
            symbolTable.AddLabel(opCode.Label.Value, _currentIp)

        let vmOpCode = opCode.Assemble(_currentIp, symbolTable, settings)
        _currentIp <- _currentIp + vmOpCode.Buffer.Length
        vmOpCode

    let assemblyFunctionBody(irFunctionBody: IrOpCode seq, symbolTable: SymbolTable, settings: AssemblerSettings) =        
        irFunctionBody
        |> Seq.map(assemblyIrOpCode(symbolTable, settings))
        |> Seq.toList
        
    let addAllocaInstruction(opCodes: IrOpCode list) =
        let allVariables = new HashSet<String>()

        // extract all local variables
        opCodes
        |> Seq.filter(fun opCode -> opCode.Type.AcceptVariable())
        |> Seq.iter(fun opCode -> 
            opCode.Operands
            |> Seq.iter(fun operand ->
                match operand.Value with
                | :? String -> allVariables.Add(operand.Value.ToString()) |> ignore
                | _ -> ()
            )
        )
        
        // create alloca instruction
        if allVariables.Count > 0 then
            let allocaInstr = new IrOpCode(IrOpCodes.Alloca)
            allocaInstr.Operands.Add(new Operand(allVariables.Count))
            allocaInstr::opCodes
        else    
            opCodes

    let generateFunctionVmOpCodes(symbolTable: SymbolTable, settings: AssemblerSettings) (irFunction: IrFunction) =
        symbolTable.StartFunction()
        
        // the analyzed function is a symbol, this will ensure that instruction like call foo, will be correctly assembled
        symbolTable.AddLabel(irFunction.Name, _currentIp)

        // add alloca instruction to compute stack space
        let fullBody = addAllocaInstruction(irFunction.Body |> Seq.toList)
                            
        // proceed to assemble VM opcodes        
        {Body=assemblyFunctionBody(fullBody, symbolTable, settings)}

    let orderFunctions(functions: List<IrFunction>, settings: AssemblerSettings) =        
        let entryPointFunction = functions |> Seq.find(fun f -> f.Name.Equals("main", StringComparison.OrdinalIgnoreCase))
        
        let otherFunctions =
            if settings.ReorderFunctions then   
                let rnd = new Random()
                functions 
                |> Seq.filter(fun f -> not(f.Name.Equals("main", StringComparison.OrdinalIgnoreCase)))
                |> Seq.sortBy(fun _ -> rnd.Next())
                |> Seq.toList
            else
                functions 
                |> Seq.filter(fun f -> not(f.Name.Equals("main", StringComparison.OrdinalIgnoreCase)))                
                |> Seq.toList

        entryPointFunction::otherFunctions
        
    member val Settings = new AssemblerSettings() with get

    member this.GenerateBinaryCode(functions: List<IrFunction>) =
        let symbolTable = new SymbolTable()
        
        // assemble the code
        let vmFunctions =
            orderFunctions(functions, this.Settings)
            |> Seq.map(generateFunctionVmOpCodes(symbolTable, this.Settings))
            |> Seq.toList

        // fix the offset
        symbolTable.FixPlaceholders(vmFunctions)

        vmFunctions 

    member this.Assemble(irCode: String) =
        _functions.Clear()
        _currentIp <- 0
        
        // generate AST
        let astBuilder = new SacaraAstBuilder()
        let ast = astBuilder.Parse(irCode)
        parseAst(ast)

        // generate VM opcode
        {Functions=this.GenerateBinaryCode(_functions)}

