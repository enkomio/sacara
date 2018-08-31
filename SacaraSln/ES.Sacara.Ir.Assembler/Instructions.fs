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
    // Return to the caller. If there is a value on top of the stack it is pushed in the caller stack as return value
    // eg. ret
    | Ret

    // No instruction
    // eg. nop
    | Nop

    // pop two values from the stack, sum them and push the result on the stack
    // eg. add
    | Add

    // push a value in the stack. The value can be an immediate a label or a variable
    // eg. push IMM/var
    | Push

    // pop a value from the stack into the specified var
    // eg. pop var
    | Pop

    // call a managed method which offset is popped off the stack
    // eg. call
    | Call

    // call a native method which offset is popped off the stack
    // eg. ncall
    | NativeCall

    // pop a value from the stack, which is the vm address to read. Push the result value in the stack
    // eg. read
    | Read

    // pop a value from the stack, which is the native address to read. Push the result value in the stack
    // rg. nread
    | NativeRead

    // pop two values from the stack, which are the managed address and the value to write. 
    // Push the result back into the stack.
    // eg. write
    | Write

    // pop two values from the stack, which are the native address and the value to write. 
    // Push the result back into the stack.
    // eg. nwrite
    | NativeWrite

    // push in the stack the offset of the next instruction that will be executed in the VM
    // eg. getip
    | GetIp

    // jump to an offset popped from the stack
    // eg. jump
    | Jump

    // pop two values from the stack and jump to the second value is the first is less than 0
    // eg. jumpifl
    | JumpIfLess

    // pop two values from the stack and jump to the second value is the first is less or equals 0
    // eg. jumpifle
    | JumpIfLessEquals

    // pop two values from the stack and jump to the second value is the first is greaten than 0
    // eg. jumpifg
    | JumpIfGreat

    // pop two values from the stack and jump to the second value is the first is greaten or equals 0
    // eg. jumpifge
    | JumpIfGreatEquals

    // allocate a given amount of space for local variables. The argument is the number of DWORD to allocate.
    // If this instruction is invoked more than one time, the size for the local variables is updated and all 
    // the previous value are deleted
    // eg. alloca 2
    | Alloca

    // write a raw byte. This is a macro and doesn't have a corrispettive VM instruction
    // eg. byte 0xFF
    | Byte

    // write a raw word. This is a macro and doesn't have a corrispettive VM instruction
    // eg. byte 0xFFFF
    | Word

    // write a raw double word. This is a macro and doesn't have a corrispettive VM instruction
    // eg. byte 0xFFFFFFFF
    | DoubleWord

    // stop the execution of VM
    // eg. halt
    | Halt

    // pop two values from the stack, compare them, and push the result  into the stack.
    // The result will be 0 if they are equals, 1 if the first value if grathen then the second, -1 otherwise.
    // eg. cmp
    | Cmp

    // push in the stack the offset of the current VM stack location
    // eg. getsp
    | GetSp

    // pop two values from the stack, which are the managed index of the stack starting from the base and the value to write. 
    // eg. swrite
    | StackWrite

    // pop a value from the stack, which is the vm index of the stack starting from the base to read. Push the result value on top of the stack
    // eg. sread
    | StackRead

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
    | VmCall
    | VmNativeCall
    | VmRead
    | VmNativeRead
    | VmWrite
    | VmNativeWrite
    | VmGetIp
    | VmJump
    | VmJumpIfLess
    | VmJumpIfLessEquals
    | VmJumpIfGreat
    | VmJumpIfGreatEquals
    | VmAlloca
    | VmHalt
    | VmCmp
    | VmGetSp
    | VmStackWrite
    | VmStackRead

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