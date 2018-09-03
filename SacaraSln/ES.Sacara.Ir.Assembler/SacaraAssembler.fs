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
        |> List.map(fun vmOpCode -> vmOpCode.ToString())
        |> fun l -> String.Join(Environment.NewLine, l)

type SacaraAssembler() =
    let mutable _currentFunction = new IrFunction(String.Empty)
    let mutable _functions = new List<IrFunction>()
    let mutable _currentLabel: String option = None
    let mutable _currentIp = 0
    let mutable _random = new Random()

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
            addOperand(new IrOpCode(if callType.Native then IrOpCodes.NativeCall else IrOpCodes.Call))
        | Read readType ->
            addOperand(new IrOpCode(if readType.Native then IrOpCodes.NativeRead else IrOpCodes.Read))
        | Write writeType ->
            addOperand(new IrOpCode(if writeType.Native then IrOpCodes.NativeWrite else IrOpCodes.Write))
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

            addOperand(new IrOpCode(opCode))
        | Jump ->
            addOperand(new IrOpCode(IrOpCodes.Jump))
        | Empty -> ()
        | Alloca ->
            addOperand(new IrOpCode(IrOpCodes.Alloca))
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
        | GetSp ->
            addOperand(new IrOpCode(IrOpCodes.GetSp))
        | StackRead ->
            addOperand(new IrOpCode(IrOpCodes.StackRead))
        | StackWrite ->
            addOperand(new IrOpCode(IrOpCodes.StackWrite))

    let parseAst(ast: Program) =
        match ast with
        | Program sl -> sl |> List.iter(parseStatement)

    let encryptVmOpCode(vmOpCode: VmOpCode) =
        let n = _random.Next(0,10)
        if [3; 8] |> List.contains(n) |> not then
            let numericOpCode = BitConverter.ToUInt16(vmOpCode.VmOp, 0)
            let mutable encNumericOpCode = numericOpCode ^^^ uint16 0x5B ^^^ uint16(vmOpCode.Offset + vmOpCode.VmOp.Length)
            
            // clear flags
            encNumericOpCode <- encNumericOpCode &&& uint16 0xFFF

            // set encrypted flag
            encNumericOpCode <- encNumericOpCode ||| uint16 0x8000
            
            let encOpCode = BitConverter.GetBytes(encNumericOpCode)

            Array.Copy(encOpCode, vmOpCode.Buffer, encOpCode.Length)
            vmOpCode.SyncOpAndOperandsWithBuffer()

    let assemblyIrOpCode(symbolTable: SymbolTable, settings: AssemblerSettings) (opCode: IrOpCode) =
        if opCode.Label.IsSome then
            symbolTable.AddLabel(opCode.Label.Value, _currentIp)

        let vmOpCode = opCode.Assemble(_currentIp, symbolTable, settings)

        // encrypt the opcode if necessary
        if settings.RandomlyEncryptOpCode then  
            encryptVmOpCode(vmOpCode)            

        _currentIp <- _currentIp + vmOpCode.Buffer.Length
        vmOpCode

    let assemblyFunctionBody(irFunctionBody: IrOpCode seq, symbolTable: SymbolTable, settings: AssemblerSettings) =        
        irFunctionBody
        |> Seq.map(assemblyIrOpCode(symbolTable, settings))
        |> Seq.toList
        
    let addAllocaInstruction(symbolTable: SymbolTable, opCodes: IrOpCode list) =
        let allVariables = new HashSet<String>()

        // extract all local variables
        let opCodeAcceptingVariables = [
            IrOpCodes.Push
            IrOpCodes.Pop
        ]
        opCodes
        |> Seq.filter(fun opCode -> opCodeAcceptingVariables |> List.contains opCode.Type)
        |> Seq.iter(fun opCode -> 
            opCode.Operands
            |> Seq.iter(fun operand ->
                match operand.Value with
                | :? String ->
                    if not(symbolTable.IsLabel(operand.Value.ToString())) then
                        allVariables.Add(operand.Value.ToString()) |> ignore
                | _ -> ()
            )
        )
        
        // create alloca instruction
        if allVariables.Count > 0 then
            let pushInstr = new IrOpCode(IrOpCodes.Push)
            pushInstr.Operands.Add(new Operand(allVariables.Count))

            let allocaInstr = new IrOpCode(IrOpCodes.Alloca)
            [pushInstr;allocaInstr]@opCodes
        else    
            opCodes

    let generateFunctionVmOpCodes(symbolTable: SymbolTable, settings: AssemblerSettings) (irFunction: IrFunction) =
        symbolTable.StartFunction()
        
        // the analyzed function is a symbol, this will ensure that instruction like call foo, will be correctly assembled
        symbolTable.AddLabel(irFunction.Name, _currentIp)

        // add alloca instruction to compute stack space
        let fullBody = addAllocaInstruction(symbolTable, irFunction.Body |> Seq.toList)
                            
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

    let addLabelNamesToSymbolTable(symbolTable: SymbolTable, functions: List<IrFunction>) =        
        functions
        |> Seq.iter(fun irFunction -> 
            symbolTable.AddLabelName(irFunction.Name)
            irFunction.Body
            |> Seq.filter(fun irOpCode -> irOpCode.Label.IsSome)
            |> Seq.iter(fun irOpCode -> symbolTable.AddLabelName(irOpCode.Label.Value))
        )
        
    member val Settings = new AssemblerSettings() with get

    member this.GenerateBinaryCode(functions: List<IrFunction>) =
        let symbolTable = new SymbolTable()

        // add all function names and labels to the symbol table, in order to be 
        // able to correctly assemble specific VM opCode
        addLabelNamesToSymbolTable(symbolTable, functions)
                
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

