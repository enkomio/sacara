/// This simple program produce a text output with VM opcodes. 
/// The generated text can be included in the sacara ir assembler
namespace GenerateVmOpCodes

open System
open System.Collections.Generic
open System.Text
open ES.Sacara.Ir.Assembler
open Microsoft.FSharp.Reflection

module Program =

    [<EntryPoint>]
    let main argv = 
        let sb = new StringBuilder()
        let generatedOpCodes = new List<Int32>()
        let rnd = new Random()
        
        FSharpType.GetUnionCases(typeof<VmOpCodes>)
        |> Array.iter(fun case ->            
            let bytes = new StringBuilder()
            for j=1 to rnd.Next(2, 6) do
                let opCode = rnd.Next(10, 65535)
                if not(generatedOpCodes.Contains(opCode)) then
                    generatedOpCodes.Add(opCode)
                    bytes.AppendFormat("; 0x{0}", opCode.ToString("X")) |> ignore
            sb.AppendFormat("({0}, [{1}])", case.Name, bytes.ToString().Substring(2)).AppendLine() |> ignore
        )

        Console.WriteLine(sb.ToString())
        Console.WriteLine("PRess enter to exit...")
        Console.ReadLine() |> ignore
        0
