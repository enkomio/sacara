namespace Sacara.EndToEndTests

open System
open System.IO
open System.Reflection
open ES.Sacara.Ir.Assembler
open ES.Sacara.Ir.Core
open ES.SacaraVm

module internal Utility =
    let _curDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
    let assemblerSettings = new AssemblerSettings()    

    let private getScriptSrc(scriptFile: String) =        
        let scriptPath = Path.Combine(_curDir, "TestSources", scriptFile)
        File.ReadAllText(scriptPath)

    let private getSacaraVmDll() =
        // check if it was specified an input SacaraDll
        let args = Environment.GetCommandLineArgs()
        if args.Length > 1
        then args.[1]
        else Path.Combine("..", "..", "..", "Debug", "SacaraVm.dll")

    let private assembleCode(code: String) =        
        let assembler = new SacaraAssembler(assemblerSettings)
        let vmCode = assembler.Assemble(code)
        vmCode.Functions.[0].Body

    let executeScriptWithArg(scriptFile: String, args: Int32 array) =
        Console.WriteLine("Execute script: {0}", scriptFile)
        let sacaraVm = new SacaraVm(getSacaraVmDll())

        // assembly source
        let script = getScriptSrc(scriptFile)
        let assembler = new SacaraAssembler(assemblerSettings)
        let vmCode = assembler.Assemble(script)

        // set arguments
        args
        |> Array.iteri(fun i arg ->
            sacaraVm.LocalVarSet(i, arg)
        )

        // run code and get result
        sacaraVm.Run(vmCode)
        int32 <| sacaraVm.LocalVarGet(args.Length)

    let executeScript(scriptFile: String) =
        executeScriptWithArg(scriptFile, Array.empty<Int32>)

    let assembleInstructionWithArg(irInstruction: IrInstruction, vmInstruction: VmInstruction, arg: String) =
        Console.WriteLine("Assemble instruction: {0} {1}, VM: {2}", irInstruction, arg, vmInstruction)
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

