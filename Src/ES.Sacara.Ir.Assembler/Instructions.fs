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

    // pop from the stack two values, that are the offset of the method to call and the number of arguments to push in the
    // ne stack frame. Once done the setup of the new stack frame invoke the code at the specified offset.
    // eg. call
    | Call

    // call a native method, it accepts three arguments which are popped from the stack
    //    - the first one is the absolute native address that we jump to
    //    - the second parameter is the number of arguments that must be popped out from the managed stack and pushed into the native stack
    //    - the third and final one is a flag stating if the stack must be cleared (1) or not (0) after the invokation 
    // After the code return, the value of the EAX register is pushed into the managed stack.
    // eg. ncall
    | NativeCall

    // pop a value from the stack, which is the vm IP address to read. Push the result value in the stack. It read a byte.
    // eg. read
    | Read

    // pop a value from the stack, which is the native address to read. Push the result value in the stack
    // rg. nread
    | NativeRead

    // pop two values from the stack, which are the managed IP offset and the value (a byte) to write.
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

    // pop a values from the stack and jump to it if the sign flag is set
    // eg. jumpifl
    | JumpIfLess

    // pop a values from the stack and jump to it if the sign flag or the zero flag are set
    // eg. jumpifle
    | JumpIfLessEquals

    // pop a values from the stack and jump to it if the sign flag is not set
    // eg. jumpifg
    | JumpIfGreat

    // pop a values from the stack and jump to it if the sign flag or the zero flag are not set
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

    // pop two values from the stack and compare them by considering an unsigned comparison.
    // The result will modify the VM status flags Zero and Carry.
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

    // pop two values from the stack, sub them and push the result on the stack
    // eg. sub
    | Sub

    // pop two values from the stack, multiply them and push the result on the stack
    // eg. mul
    | Mul

    // pop two values from the stack, divide the first argument with the second and push the result on the stack
    // eg. div
    | Div

    // pop two values from the stack, AND the first value with the second one and push the result on the stack
    // eg. and
    | And

    // pop two values from the stack, shift to right the first argument for a number of time equals to the second argument and push the result back into the stack
    // eg. shiftr
    | ShiftRight

    // pop two values from the stack, shift to left the first argument for a number of time equals to the second argument and push the result back into the stack
    // eg. shiftl
    | ShiftLeft

    // pop two values from the stack, OR the first value with the second one and push the result on the stack
    // eg. or
    | Or

    // pop a value from the stack, NOT it and push the result on the stack
    // eg. not
    | Not

    // pop two values from the stack, XOR the first value with the second one and push the result on the stack
    // eg. xor
    | Xor

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
    | VmJumpIfGreat
    | VmJumpIfGreatEquals
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