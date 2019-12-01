namespace Sacara.EndToEndTests

open System
open System.IO
open System.Reflection
open ES.Sacara.Ir.Assembler
open ES.Sacara.Ir.Core
open ES.SacaraVm

[<AutoOpen>]
module internal Utility =
    let scriptDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "TestSources")
    let assemblerSettings = new AssemblerSettings()    

    let getScriptFullPath(scriptFile: String) =
        let selfContainedScript = Path.Combine(scriptDir, "SelfContained", scriptFile)
        if File.Exists(selfContainedScript) then selfContainedScript
        else Path.Combine(scriptDir, "Custom", scriptFile)

    let getScriptSrc(scriptFile: String) = 
        File.ReadAllText(getScriptFullPath(scriptFile))

    let private getSacaraVmDll() =
        // check if it was specified an input SacaraDll
        let args = Environment.GetCommandLineArgs()
        if args.Length > 1 then args.[1]
        else 
            let dllName = "SacaraVm.dll"
            let debugPath = Path.Combine("..", "..", "..", "Debug", dllName)
            if not <| File.Exists(debugPath) then
                let releasePath = Path.Combine("..", "..", "..", "Release", dllName)
                if not <| File.Exists(releasePath) then
                    failwith "Unable to find the SacarVm Dll in the Debug or Release path. Please first compile it."
                else
                    releasePath
            else
                debugPath

    let private assembleCode(code: String) =        
        let assembler = new SacaraAssembler(assemblerSettings)
        let vmCode = assembler.Assemble(code)
        vmCode.Functions.[0].Body

    let assertTest(testResult: Int32, expectedResult: Int32) =
        if testResult <> expectedResult then
            failwith(
                String.Format(
                    "Test returned value {0} instead of the expectedvalue {1}",
                    testResult, 
                    expectedResult
                )
            )

    let executeScriptWithArg(scriptFile: String, args: Int32 array) =
        Console.WriteLine("Execute script: {0}", scriptFile)        
        let sacaraVm = new SacaraVm(getSacaraVmDll())

        // assembly source
        let script = getScriptSrc(scriptFile)
        let assembler = new SacaraAssembler(assemblerSettings)
        let vmCode = assembler.Assemble(script)

        // print code for debugging
        Console.WriteLine(vmCode)

        // run code and get result
        sacaraVm.Run(vmCode, args)
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

    let runTests(pattern: String) =
        Assembly.GetExecutingAssembly().GetTypes()
        |> Seq.find(fun t -> t.Name.EndsWith(pattern))
        |> fun t -> t.GetMethods()
        |> Seq.filter(fun m -> m.Name.StartsWith("Test"))
        |> Seq.iter(fun m -> m.Invoke(null, Array.empty) |> ignore)