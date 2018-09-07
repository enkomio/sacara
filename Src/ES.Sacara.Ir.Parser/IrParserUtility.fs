namespace ES.Sacara.Ir.Parser

open System
open ES.Sacara.Ir.Parser.IrAst
open System.Text

module internal IrParserUtility =    

    let emptyStatement() = 
        Empty

    let number(num: Int32) =
        Number num

    let identifier(name: String) =
        Identifier name

    let push(expr: Expression) =
        Push {Operand=expr}

    let pop(identifier: String) =
        Pop {Identifier=identifier}

    let procedureDefinition(name: String, statements: Statement list) =
        Procedure {Name=name; Body=statements}

    let label(name: String, statement: Statement) =
        Label {Name=name.Substring(0, name.Length-1); Statement=statement}

    let call() =
        Call {Native=false}

    let callNative() =
        Call {Native=true}

    let read() =
        Read {Native=false}

    let readNative() =
        Read {Native=true}

    let write() =
        Write {Native=false}

    let writeNative() =
        Write {Native=true}

    let jumpIf(equals: Boolean, less: Boolean) =
        JumpIf {JumpIfEquals=equals; JumpIfLess=less}

    let jump() =
        Jump

    let alloca() =
        Alloca

    let memoryByte(values: Int32 list list) =
        Byte (values |> List.concat)

    let memoryWord(values: Int32 list) =
        let canonicalizedValues = 
            values 
            |> List.map(fun value -> if value > 0XFFFF then 0xFFFF else value)

        Word canonicalizedValues

    let memoryDword(values: Int32 list) =
        DoubleWord values

    let getStringBytes(str: String) =
        Encoding.UTF8.GetBytes(str)
        |> Seq.map(fun b -> int32 b)
        |> Seq.toList

    let nop() =
        Nop

    let getIp() =
        GetIp

    let add() =
        Add

    let ret() =
        Ret

    let halt() =
        Halt

    let compare() =
        Cmp

    let getSp() =
        GetSp

    let stackWrite() =
        StackWrite

    let stackRead() =
        StackRead

    let sub() =
        Sub

    let mul() =
        Mul

    let div() =
        Div

    let bitAnd() =
        And

    let shiftRight() =
        ShiftRight

    let shiftLeft() =
        ShiftLeft

    let bitOr() =
        Or

    let bitNot() =
        Not

    let xor() =
        Xor
