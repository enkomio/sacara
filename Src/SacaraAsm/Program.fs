namespace ES.Sacara.Asm

open System
open System.Text
open System.IO
open Argu
open System.Diagnostics
open ES.Sacara.Ir.Assembler
open System.Reflection
open ES.Fslog
open ES.Fslog.Loggers

module Program =    
    type CLIArguments =
        | Encrypt_OpCodes
        | Encrypt_Operands
        | Add_Junk
        | Reorder_Functions
        | Multiple_OpCodes
        | Gen_AsmLang
        | Gen_CLang
        | Gen_Intermediate
        | Output of filename: String
        | [<MainCommand; ExactlyOnce; Last>] SourceFile of filename: String
        | Version
        | Verbose
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Encrypt_OpCodes -> "encrypt the generated opcodes."
                | Encrypt_Operands -> "encrypt the instruction operands."
                | Add_Junk -> "insert junk instructions in the original source code."
                | Reorder_Functions -> "the function order will be set casually inside the final code."
                | Multiple_OpCodes -> "use different byte representation for the same opcode."
                | Gen_AsmLang -> "generate a MASM syntax file of the geenrated opcode."
                | Gen_CLang -> "generate a C syntax file of the geenrated opcode."
                | Gen_Intermediate -> "generate a text file that is the textual representation of the generated code."
                | SourceFile _ -> "the SIL source code filename."
                | Output _ -> "the output file to use for the code generation."
                | Verbose _ -> "print verbose messages."
                | Version _ -> "print the Sacara Assembler version."

    let printBanner() =
        Console.ForegroundColor <- ConsoleColor.Cyan        
        let banner = "-=[ Sacara SIL Assembler ]=-"
        let year = if DateTime.Now.Year = 2018 then "2018" else String.Format("2018-{0}", DateTime.Now.Year)
        let copy = String.Format("Copyright (c) {0} Antonio Parata - @s4tan{1}", year, Environment.NewLine)
        Console.WriteLine(banner.PadLeft(abs(banner.Length - copy.Length) / 2 + banner.Length))
        Console.WriteLine(copy)
        Console.ResetColor()

    let printUsage(body: String) =
        Console.WriteLine(body)

    let printError(errorMsg: String) =
        Console.ForegroundColor <- ConsoleColor.Red
        Console.WriteLine(errorMsg)
        Console.ResetColor()

    let printVersion() =
        let version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion
        Console.WriteLine("Sacara assembler version: {0}", version)

    let getMasmCode(code: IrAssemblyCode) =
        let sb = new StringBuilder()
        let mutable index = 0
        code.Functions
        |> Seq.iter(fun irFunction ->
            irFunction.Body
            |> Seq.iter(fun opCode ->
                let bytesTring = 
                    opCode.Buffer
                    |> Seq.map(fun b -> b.ToString("X") + "h") 
                    |> Seq.map(fun b -> 
                        if 
                            b.StartsWith("a", StringComparison.OrdinalIgnoreCase) || b.StartsWith("b", StringComparison.OrdinalIgnoreCase) ||
                            b.StartsWith("c", StringComparison.OrdinalIgnoreCase) || b.StartsWith("d", StringComparison.OrdinalIgnoreCase) ||
                            b.StartsWith("e", StringComparison.OrdinalIgnoreCase) || b.StartsWith("f", StringComparison.OrdinalIgnoreCase)
                        then "0" + b
                        else b
                    )
                sb.AppendFormat("code_{0} BYTE {1} ; {2}", index.ToString("0000"), String.Join(",", bytesTring).PadRight(40), opCode.ToString()).AppendLine() |> ignore
                index <- index + 1
            )
        )
        sb.ToString()

    let getCCode(code: IrAssemblyCode) =
        let sb = new StringBuilder("uint8_t code[] = {")
        sb.AppendLine() |> ignore

        let allOpCodes =
            code.Functions
            |> Seq.map(fun irFunction -> irFunction.Body)
            |> Seq.concat
            |> Seq.toList

        allOpCodes
        |> Seq.iteri(fun i opCode ->
            let bytesTring = 
                opCode.Buffer
                |> Seq.map(fun b -> "0x" + b.ToString("X"))

            let bytes = 
                if i = allOpCodes.Length - 1
                then String.Join(",", bytesTring)
                else String.Join(",", bytesTring) + ","

            sb.AppendFormat("\t{0} // {1}", bytes.PadRight(40), opCode.ToString()).AppendLine() |> ignore
        )
        sb.Append("};") |> ignore

        sb.ToString()

    let getLogProvider(isVerbose: Boolean) =
        let logProvider = new LogProvider()    
        let logLevel = if isVerbose then LogLevel.Verbose else LogLevel.Informational
        logProvider.AddLogger(new ConsoleLogger(logLevel))
        logProvider

    [<EntryPoint>]
    let main argv = 
        printBanner()

        let parser = ArgumentParser.Create<CLIArguments>(programName = "SacaraAsm.exe")
        try            
            let results = parser.Parse(argv)
                    
            if results.IsUsageRequested then
                printUsage(parser.PrintUsage())
                0
            elif results.Contains(<@ Version @>) then
                printVersion()
                0
            else            
                let settings = 
                    new AssemblerSettings(
                        InserJunkOpCodes = results.Contains(<@ Add_Junk @>),
                        ReorderFunctions = results.Contains(<@ Reorder_Functions @>),
                        UseMultipleOpcodeForSameInstruction = results.Contains(<@ Multiple_OpCodes @>),
                        RandomlyEncryptOpCode = results.Contains(<@ Encrypt_OpCodes @>),
                        EncryptOperands = results.Contains(<@ Encrypt_Operands @>)
                    )

                let sourceFilename = results.GetResult(<@ SourceFile @>)                
                let defaultOutputFile = Path.GetFileNameWithoutExtension(sourceFilename) + ".sac"
                let outputFile = results.GetResult(<@ Output @>, defaultOutputFile)
                let createAsmFile = results.Contains(<@ Gen_AsmLang @>)
                let createCLangFile = results.Contains(<@ Gen_CLang @>)
                let createIntermediateFile = results.Contains(<@ Gen_Intermediate @>)

                let logger =
                    log "SacaraAssembler"
                    |> info "AsmCreated" "Create ASM file: {0}"
                    |> info "CCreated" "Create C file: {0}"
                    |> info "OutputCreated" "VM code written to file: {0}"
                    |> info "IntermediateCreated"  "Create intermediate file: {0}"
                    |> buildAndAdd(getLogProvider(results.Contains(<@ Verbose @>)))

                if not <| File.Exists(sourceFilename) then
                    printError(String.Format("Filename '{0}' not found", sourceFilename))
                    1
                else
                    let sourceCode = File.ReadAllText(sourceFilename)
                    let assembler = new SacaraAssembler(settings)   
                    let irCode = assembler.Assemble(sourceCode)

                    if createAsmFile then
                        let asmFile = Path.GetFileNameWithoutExtension(outputFile) + ".asm"
                        File.WriteAllText(asmFile, getMasmCode(irCode))
                        logger?AsmCreated(asmFile)

                    if createCLangFile then
                        let asmFile = Path.GetFileNameWithoutExtension(outputFile) + ".c"
                        File.WriteAllText(asmFile, getCCode(irCode))
                        logger?CCreated(asmFile)

                    if createIntermediateFile then
                        let intermediateFile = Path.GetFileNameWithoutExtension(outputFile) + ".sil"
                        File.WriteAllText(intermediateFile, irCode.ToString())
                        logger?IntermediateCreated(intermediateFile)

                    File.WriteAllBytes(outputFile, irCode.GetBuffer())
                    logger?OutputCreated(outputFile)
                    0
        with 
            | :? ArguParseException ->
                printUsage(parser.PrintUsage())   
                1
            | e ->
                printError(e.ToString())
                1