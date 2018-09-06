namespace ES.Sacara

open System
open System.Text
open ES.Sacara.Ir.Assembler

module Program =
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
            push 12
            push 44
            cmp
            push first_operand_is_less
            jumpifl                     /* jump if 44 < 12 */
            push 1
            pop result_var
        first_operand_is_less:
            push 0
            pop result_var
            halt
        endp
        """
        let assembler = new SacaraAssembler()
        assembler.Settings.RandomlyEncryptOpCode <- false
        let code = assembler.Assemble(irCode)

        let vmListing = code.ToString()

        Console.WriteLine("MASM:")
        Console.WriteLine(getMasmCode(code))

        Console.WriteLine("C:")
        Console.WriteLine(getCCode(code))
        0