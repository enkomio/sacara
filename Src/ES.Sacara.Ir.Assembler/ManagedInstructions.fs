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
    let createInstrcutionWitArg(irType: IrOpCodes, arg: Object) (ctx: Ctx) =
        let irOpCode = new IrOpCode(irType, ctx.Settings.UseMultipleOpcodeForSameInstruction)
        irOpCode.Operands.Add(new Operand(arg))
        ctx.CurrentFunction.Add(Some irOpCode)

    let createInstrcution(irType: IrOpCodes) (ctx: Ctx) =
        let irOpCode = new IrOpCode(irType, ctx.Settings.UseMultipleOpcodeForSameInstruction)
        ctx.CurrentFunction.Add(Some irOpCode)

// Kudos to: http://nut-cracker.azurewebsites.net/blog/2011/11/15/typeclasses-for-fsharp/
type T = T with
    static member ($) (T, arg: Int32) = createInstrcutionWitArg(IrOpCodes.Push, arg)
    static member ($) (T, arg: String) = createInstrcutionWitArg(IrOpCodes.Push, arg)

type A = A with
    static member (%) (A, arg: Int32 list) = createInstrcutionWitArg(IrOpCodes.Byte, arg)
    static member (%) (A, arg: String) = createInstrcutionWitArg(IrOpCodes.Byte, arg)

[<AutoOpen>]
module ManagedInstructions =
    let proc(procName: String) (ctx: Ctx) =
        ctx.CurrentFunction <- new List<IrOpCode option>()
        ctx.Functions.Add(procName, ctx.CurrentFunction)
        
    let endp: Ctx -> unit = fun _ -> ()
    let ret: Ctx -> unit = createInstrcution(IrOpCodes.Ret)
    let nop: Ctx -> unit = createInstrcution(IrOpCodes.Nop)
    let add: Ctx -> unit = createInstrcution(IrOpCodes.Add)
    let inline push (arg: 't) = (T $ arg)
    let pop(var: String) = createInstrcutionWitArg(IrOpCodes.Pop, var)
    let call: Ctx -> unit = createInstrcution(IrOpCodes.Call)
    let ncall: Ctx -> unit = createInstrcution(IrOpCodes.NativeCall)
    let read: Ctx -> unit = createInstrcution(IrOpCodes.Read)
    let nread: Ctx -> unit = createInstrcution(IrOpCodes.NativeRead)
    let write: Ctx -> unit = createInstrcution(IrOpCodes.Write)
    let nwrie: Ctx -> unit = createInstrcution(IrOpCodes.NativeWrite)
    let getip: Ctx -> unit = createInstrcution(IrOpCodes.GetIp)
    let jump: Ctx -> unit = createInstrcution(IrOpCodes.Jump)
    let jumpifl: Ctx -> unit = createInstrcution(IrOpCodes.JumpIfLess)
    let jumpifle: Ctx -> unit = createInstrcution(IrOpCodes.JumpIfLessEquals)
    let jumpifg: Ctx -> unit = createInstrcution(IrOpCodes.JumpIfGreater)
    let jumpifge: Ctx -> unit = createInstrcution(IrOpCodes.JumpIfGreaterEquals)
    let alloca: Ctx -> unit = createInstrcution(IrOpCodes.Alloca)    
    let inline byte(arg: 'a) = (A % arg)
    let word(word: Int32) = createInstrcutionWitArg(IrOpCodes.Word, uint16 word)    
    let dword(dword: Int32) = createInstrcutionWitArg(IrOpCodes.DoubleWord, dword)
    let halt: Ctx -> unit = createInstrcution(IrOpCodes.Halt)
    let cmp: Ctx -> unit = createInstrcution(IrOpCodes.Cmp)
    let getsp: Ctx -> unit = createInstrcution(IrOpCodes.GetSp)
    let swrite: Ctx -> unit = createInstrcution(IrOpCodes.StackWrite)
    let sread: Ctx -> unit = createInstrcution(IrOpCodes.StackRead)
    let sub: Ctx -> unit = createInstrcution(IrOpCodes.Sub)
    let mul: Ctx -> unit = createInstrcution(IrOpCodes.Mul)
    let div: Ctx -> unit = createInstrcution(IrOpCodes.Div)
    let _and: Ctx -> unit = createInstrcution(IrOpCodes.And)
    let sright: Ctx -> unit = createInstrcution(IrOpCodes.ShiftRight)
    let sleft: Ctx -> unit = createInstrcution(IrOpCodes.ShiftLeft)
    let _or: Ctx -> unit = createInstrcution(IrOpCodes.Or)
    let _nop: Ctx -> unit = createInstrcution(IrOpCodes.Not)
    let xor: Ctx -> unit = createInstrcution(IrOpCodes.Xor)
    let nor: Ctx -> unit = createInstrcution(IrOpCodes.Nor)
    let setip: Ctx -> unit = createInstrcution(IrOpCodes.SetIp)
    let setsp: Ctx -> unit = createInstrcution(IrOpCodes.SetSp)