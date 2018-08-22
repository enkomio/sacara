namespace ES.Sacara.Ir.Assembler

open System

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

    // Allocate a given amount of stakc space. The argument is the number of DWORD to allocate in the managed stack
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

module Instructions =
    let bytes = 
        [
            // each VM opcode can have different format. This was generate with the GenerateVmOpCodes utility
            (VmRet, [0x8550; 0xDE86; 0xF6D6])
            (VmNop, [0x12E8; 0x3E64; 0x6F97])
            (VmAdd, [0x698B; 0x63D7; 0xB2A4; 0x8B84; 0x68E0])
            (VmPushImmediate, [0xE1D6; 0x5481; 0x506C; 0xF8F8])
            (VmPushVariable, [0x2FD5; 0x2A20; 0xB7D1])
            (VmPop, [0x4578; 0x48DD])
            (VmCallImmediate, [0x4E7F; 0xED84; 0xF7F6; 0x86B8; 0x6A9E])
            (VmCallVariable, [0x1D1B; 0x8AAC; 0x2FC1])
            (VmNativeCallImmediate, [0xA68E; 0xFF15; 0x3397; 0xF1E9; 0x7CC8])
            (VmNativeCallVariable, [0xC149; 0xE227; 0xBF7C; 0x28A7; 0x8B0B])
            (VmReadImmediate, [0xB01A; 0x8F28])
            (VmReadVariable, [0x92D9; 0xDB14; 0xFB34])
            (VmNativeReadImmediate, [0xCD7D; 0xDDB8])
            (VmNativeReadVariable, [0x221D; 0x292D; 0xEFE4; 0x8211])
            (VmWriteImmediate, [0xE2D2; 0xF938; 0x34FF; 0x6E73; 0xDE32])
            (VmWriteVariable, [0xFE80; 0xADF2; 0x5156; 0xC56A])
            (VmNativeWriteImmediate, [0xB30B; 0x2F50; 0xF686])
            (VmNativeWriteVariable, [0x85FB; 0x2040])
            (VmGetIp, [0x176F; 0x3D73; 0x68D8])
            (VmJumpImmediate, [0xD7C4; 0xA298])
            (VmJumpVariable, [0x5DA2; 0x6242])
            (VmJumpIfLessImmediate, [0xF525; 0xDD01])
            (VmJumpIfLessVariable, [0x20F; 0xFAB5])
            (VmJumpIfLessEqualsImmediate, [0xDE76; 0xE8EE])
            (VmJumpIfLessEqualsVariable, [0xBA32; 0xACD8; 0xB176; 0xB199])
            (VmJumpIfGreatImmediate, [0x4CC; 0xCD6E; 0x7CDA; 0x4832; 0xFDC2])
            (VmJumpIfGreatVariable, [0xBF13; 0x2B75; 0x1E6; 0xCA74; 0xD87E])
            (VmJumpIfGreatEqualsImmediate, [0x73BA; 0xB233; 0x217E])
            (VmJumpIfGreatEqualsVariable, [0x4C97; 0x8099; 0x2369; 0xF8E3])
            (VmAlloca, [0x7453; 0xBAFF; 0x5C3E; 0xB855])
            (VmByte, [0x1819; 0x35C0])
            (VmWord, [0x660C; 0xB1DE; 0x65F7; 0x8BD9; 0x87A3])
            (VmDoubleWord, [0xA5F; 0x25C8])
        ] |> Map.ofList

