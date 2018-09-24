namespace ES.Sacara.Ir.Core

open System
open System.Reflection
open System.Collections.Generic
open Newtonsoft.Json
open System.IO
open Microsoft.FSharp.Reflection

[<Struct>]
// the operand direction is the same as the one specified in INTEL syntax, that is: <op> <dst>, <src>
type IrOpCodes =
    | Ret
    | Nop
    | Add
    | Push
    | Pop
    | Call
    | NativeCall
    | Read
    | NativeRead
    | Write
    | NativeWrite
    | GetIp
    | Jump
    | JumpIfLess
    | JumpIfLessEquals
    | JumpIfGreater
    | JumpIfGreaterEquals
    | Alloca
    | Byte
    | Word
    | DoubleWord
    | Halt
    | Cmp
    | GetSp
    | StackWrite
    | StackRead
    | Sub
    | Mul
    | Div
    | And
    | ShiftRight
    | ShiftLeft
    | Or
    | Not
    | Xor
    | Nor
    | SetIp
    | SetSp

// these are the op codes of the VM
type VmOpCodes =    
    | VmRet
    | VmNop
    | VmAdd
    | VmPushImmediate
    | VmPushVariable
    | VmPop
    | VmCall
    | VmJump
    | VmAlloca
    | VmHalt
    | VmCmp
    | VmStackWrite
    | VmStackRead
    | VmJumpIfLess
    | VmJumpIfLessEquals
    | VmJumpIfGreater
    | VmJumpIfGreaterEquals
    | VmRead
    | VmWrite
    | VmGetIp
    | VmGetSp    
    | VmNativeRead    
    | VmNativeWrite
    | VmNativeCall
    | VmByte
    | VmWord
    | VmDoubleWord
    | VmSub
    | VmMul
    | VmDiv
    | VmAnd
    | VmShiftRight
    | VmShiftLeft
    | VmOr
    | VmNot
    | VmXor
    | VmNor
    | VmSetIp
    | VmSetSp

module Instructions =
    type VmOpCodeItem() =
        member val Name = String.Empty with get, set
        member val Bytes = new List<Byte array>() with get, set
        member val OpCodes = new List<Int32>() with get, set

    let readVmOpCodeBinding() =
        let currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        let filename = Path.Combine(currentDir, "vm_opcodes.json")
        let json = File.ReadAllText(filename)
        JsonConvert.DeserializeObject<List<VmOpCodeItem>>(json)
        |> Seq.map(fun item ->
            let vmOpCode =
                FSharpType.GetUnionCases typeof<VmOpCodes>
                |> Array.find(fun case -> case.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase))
                |> fun case -> FSharpValue.MakeUnion(case,[||]) :?> VmOpCodes
            let bytes = item.OpCodes |> Seq.toList
            (vmOpCode, bytes)
        )
        |> Map.ofSeq        

    // each VM opcode can have different format. The file was generate with the GenerateVmOpCodes utility
    let bytes = readVmOpCodeBinding()