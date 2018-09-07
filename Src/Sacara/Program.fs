namespace ES.Sacara

open System
open System.Text
open Argu
open System.Diagnostics
open ES.Sacara.Ir.Assembler
open System.Reflection

module Program =
    type CLIArguments =
        | Encrypt_OpCodes
        | Encrypt_Operands
        | Use_Multiple_OpCodes_Representation
        | Gen_AsmLang
        | Gen_CLang
        | Version
        | Verbose
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Encrypt_OpCodes -> "encrypt the generated opcodes."
                | Encrypt_Operands -> "encrypt the instruction operands."
                | Use_Multiple_OpCodes_Representation -> "use different byte representation for the same opcode."
                | Gen_AsmLang -> "generate a MASM syntax file of the geenrated opcode."
                | Gen_CLang -> "generate a C syntax file of the geenrated opcode."
                | Verbose _ -> "print verbose messages."
                | Version _ -> "print the Shed version."

    let printBanner() =
        Console.ForegroundColor <- ConsoleColor.Cyan        
        let banner = "-=[ Sacara IL Assembler ]=-"
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

    [<EntryPoint>]
    let main argv = 
        let irCode = """
        proc main  
            /* invoke the native function pointer stored in local var0 */            
            push 0
            push 0
            push local0
            ncall
            halt
        endp
        """
        let assembler = new SacaraAssembler()
        assembler.Settings.RandomlyEncryptOpCode <- true
        let code = assembler.Assemble(irCode)
        
        Console.WriteLine("MASM:")
        Console.WriteLine(getMasmCode(code))

        Console.WriteLine("C:")
        Console.WriteLine(getCCode(code))
        0