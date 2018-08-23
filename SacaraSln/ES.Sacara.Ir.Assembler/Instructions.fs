namespace ES.Sacara.Ir.Assembler

open System
open System.Reflection
open System.Collections.Generic
open Newtonsoft.Json
open System.IO
open Microsoft.FSharp.Reflection

[<Struct>]
// the operand direction is the same as the one specified in INTEL syntax, that is: <op> <dst>, <src>
type IrOpCodes =
    // Return to the caller. The value on top of the stack is the function result and is pushed into the caller stack    
    // eg. ret
    | Ret

    // No instruction
    // eg. nop
    | Nop

    // pop two value from the stack, sum them and push the result on the stack
    // eg. add
    | Add

    // push a value in the stack
    // eg. push IMM/var
    | Push

    // pop a value from the stack into the specified var
    // eg. pop var
    | Pop

    // call a managed method specified by offset or variable
    // eg. call IMM/label
    | Call

    // call a native method specified by absolute address of variable
    // eg. ncall IMM/var
    | NativeCall

    // read a DWORD from the managed memory of the code segment and push the result into the stack
    // eg. read IMM/label
    | Read

    // read a memory from the native process memory
    // rg. nread IMM/var
    | NativeRead

    // write a DWORD into the managed memory of the code segment
    // eg. write IMM/label, IMM
    | Write

    // write a value into the native process memory
    // eg. nwrite IMM/var, IMM
    | NativeWrite

    // push in the stack the offset of the next instruction that will be executed in the VM
    // eg. getip
    | GetIp

    // Jump to the specified off in the VM
    // eg. jump label
    | Jump

    // Jump if the value on the stack is less than 0
    // eg. jumpifl label
    | JumpIfLess

    // Jump if the value on the stack is less or equals to 0
    // eg. jumpifle label
    | JumpIfLessEquals

    // Jump if the value on the stack is great than 0
    // eg. jumpifg label
    | JumpIfGreat

    // Jump if the value on the stack is great or equals to 0
    // eg. jumpifge label
    | JumpIfGreatEquals

    // Allocate a given amount of stack space. The argument is the number of DWORD to allocate in the managed stack
    // eg. alloca 2
    | Alloca

    // Write a raw byte
    // eg. byte 0xFF
    | Byte

    // Write a raw word
    // eg. byte 0xFFFF
    | Word

    // Write a raw double word
    // eg. byte 0xFFFFFFFF
    | DoubleWord

    // Stop the VM
    // eg. halt
    | Halt

    // Compare two values from the stack and set the appropriate flags
    // eg. cmp
    | Cmp

    with
        member this.AcceptVariable() =
            match this with
            | Push
            | Pop
            | NativeCall
            | NativeRead
            | NativeWrite -> true
            | _ -> false


        member this.AcceptLabel() =
            match this with
            | Call
            | Read
            | Write
            | Jump
            | JumpIfLess
            | JumpIfLessEquals
            | JumpIfGreat
            | JumpIfGreatEquals -> true
            | _ -> false

// these are the op codes of the VM. 
// The operand size is 2 bytes if it is a varaible (the meaning if the offset of the variable in the stack).
// If the operand is an immediate the size is 4 bytes.
type VmOpCodes =    
    | VmRet
    | VmNop
    | VmAdd
    | VmPushImmediate
    | VmPushVariable
    | VmPop
    | VmCallImmediate
    | VmCallVariable
    | VmNativeCallImmediate
    | VmNativeCallVariable
    | VmReadImmediate
    | VmReadVariable
    | VmNativeReadImmediate
    | VmNativeReadVariable
    | VmWriteImmediate
    | VmWriteVariable
    | VmNativeWriteImmediate
    | VmNativeWriteVariable
    | VmGetIp
    | VmJumpImmediate
    | VmJumpVariable
    | VmJumpIfLessImmediate
    | VmJumpIfLessVariable
    | VmJumpIfLessEqualsImmediate
    | VmJumpIfLessEqualsVariable
    | VmJumpIfGreatImmediate
    | VmJumpIfGreatVariable
    | VmJumpIfGreatEqualsImmediate
    | VmJumpIfGreatEqualsVariable
    | VmAlloca
    | VmByte
    | VmWord
    | VmDoubleWord
    | VmHalt
    | VmCmp

module Instructions =
    type VmOpCodeItem() =
        member val Name = String.Empty with get, set
        member val Bytes = new List<Int32>() with get, set

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
            let bytes = item.Bytes |> Seq.toList
            (vmOpCode, bytes)
        )
        |> Map.ofSeq        

    // each VM opcode can have different format. The file was generate with the GenerateVmOpCodes utility
    let bytes = readVmOpCodeBinding()