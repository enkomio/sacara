namespace ES.Sacara.Ir.Parser

open System
open ES.Sacara.Ir.Parser.IrAst

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

    let jumpIf(destination: Expression, equals: Boolean, less: Boolean) =
        JumpIf {Destination=destination; JumpIfEquals=equals; JumpIfLess=less}

    let jump(destination: Expression) =
        Jump {Destination=destination}

    let alloca() =
        Alloca

    let memoryByte(value: Int32) =
        if value > 255 
        then Byte 0xFF
        else Byte value

    let memoryWord(value: Int32) =
        if value > 0XFFFF
        then Word 0xFFFF
        else Word value

    let memoryDword(value: Int32) =
        DoubleWord value

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

