namespace ES.Sacara.Ir.Assembler

open System
open System.Collections.Generic
open ES.Sacara.Ir.Core

type Ctx() =
    member val Settings = new AssemblerSettings() with get, set
    member val Functions = new Dictionary<String, List<IrOpCode option>>() with get
    member val CurrentFunction = new List<IrOpCode option>() with get, set

[<AutoOpen>]
module private IrHelpers =
    let createInstructionWithArg(irType: IrInstruction, arg: Object) (ctx: Ctx) =
        let irOpCode = new IrOpCode(irType, ctx.Settings.UseMultipleOpcodeForSameInstruction)
        irOpCode.Operands.Add(new Operand(arg))
        ctx.CurrentFunction.Add(Some irOpCode)

    let createInstruction(irType: IrInstruction) (ctx: Ctx) =
        let irOpCode = new IrOpCode(irType, ctx.Settings.UseMultipleOpcodeForSameInstruction)
        ctx.CurrentFunction.Add(Some irOpCode)

// Kudos to: http://nut-cracker.azurewebsites.net/blog/2011/11/15/typeclasses-for-fsharp/
type T = T with
    static member ($) (T, arg: Int32) = createInstructionWithArg(IrInstruction.Push, arg)
    static member ($) (T, arg: String) = createInstructionWithArg(IrInstruction.Push, arg)

type A = A with
    static member (%) (A, arg: Int32 list) = createInstructionWithArg(IrInstruction.Byte, arg)
    static member (%) (A, arg: String) = createInstructionWithArg(IrInstruction.Byte, arg)

[<AutoOpen>]
module ManagedInstructions =
    let proc(procName: String) (ctx: Ctx) =
        ctx.CurrentFunction <- new List<IrOpCode option>()
        ctx.Functions.Add(procName, ctx.CurrentFunction)
        
    let endp: Ctx -> unit = fun _ -> ()
    let ret: Ctx -> unit = createInstruction(IrInstruction.Ret)
    let nop: Ctx -> unit = createInstruction(IrInstruction.Nop)
    let add: Ctx -> unit = createInstruction(IrInstruction.Add)
    let inline push (arg: 't) = (T $ arg)
    let pop(var: String) = createInstructionWithArg(IrInstruction.Pop, var)
    let call: Ctx -> unit = createInstruction(IrInstruction.Call)
    let ncall: Ctx -> unit = createInstruction(IrInstruction.NativeCall)
    let read: Ctx -> unit = createInstruction(IrInstruction.Read)
    let nread: Ctx -> unit = createInstruction(IrInstruction.NativeRead)
    let write: Ctx -> unit = createInstruction(IrInstruction.Write)
    let nwrie: Ctx -> unit = createInstruction(IrInstruction.NativeWrite)
    let getip: Ctx -> unit = createInstruction(IrInstruction.GetIp)
    let jump: Ctx -> unit = createInstruction(IrInstruction.Jump)
    let jumpifl: Ctx -> unit = createInstruction(IrInstruction.JumpIfLess)
    let jumpifle: Ctx -> unit = createInstruction(IrInstruction.JumpIfLessEquals)
    let jumpifg: Ctx -> unit = createInstruction(IrInstruction.JumpIfGreater)
    let jumpifge: Ctx -> unit = createInstruction(IrInstruction.JumpIfGreaterEquals)
    let alloca: Ctx -> unit = createInstruction(IrInstruction.Alloca)    
    let inline byte(arg: 'a) = (A % arg)
    let word(word: Int32) = createInstructionWithArg(IrInstruction.Word, uint16 word)    
    let dword(dword: Int32) = createInstructionWithArg(IrInstruction.DoubleWord, dword)
    let halt: Ctx -> unit = createInstruction(IrInstruction.Halt)
    let cmp: Ctx -> unit = createInstruction(IrInstruction.Cmp)
    let getsp: Ctx -> unit = createInstruction(IrInstruction.GetSp)
    let swrite: Ctx -> unit = createInstruction(IrInstruction.StackWrite)
    let sread: Ctx -> unit = createInstruction(IrInstruction.StackRead)
    let sub: Ctx -> unit = createInstruction(IrInstruction.Sub)
    let mul: Ctx -> unit = createInstruction(IrInstruction.Mul)
    let div: Ctx -> unit = createInstruction(IrInstruction.Div)
    let _and: Ctx -> unit = createInstruction(IrInstruction.And)
    let sright: Ctx -> unit = createInstruction(IrInstruction.ShiftRight)
    let sleft: Ctx -> unit = createInstruction(IrInstruction.ShiftLeft)
    let _or: Ctx -> unit = createInstruction(IrInstruction.Or)
    let _not: Ctx -> unit = createInstruction(IrInstruction.Not)
    let xor: Ctx -> unit = createInstruction(IrInstruction.Xor)
    let _mod: Ctx -> unit = createInstruction(IrInstruction.Mod)
    let nor: Ctx -> unit = createInstruction(IrInstruction.Nor)
    let setip: Ctx -> unit = createInstruction(IrInstruction.SetIp)
    let setsp: Ctx -> unit = createInstruction(IrInstruction.SetSp)
    let inc: Ctx -> unit = createInstruction(IrInstruction.Inc)