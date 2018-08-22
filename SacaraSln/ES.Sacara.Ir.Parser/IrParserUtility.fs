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

    let call(operand: Expression) =
        Call {Operand=operand; Native=false}

    let callNative(operand: Expression) =
        Call {Operand=operand; Native=true}

    let read(operand: Expression) =
        Read {Operand=operand; Native=false}

    let readNative(operand: Expression) =
        Read {Operand=operand; Native=true}

    let write(destOperand: Expression, srcOperand: Int32) =
        Write {DestOperand=destOperand; SourceOperand=srcOperand; Native=false}

    let writeNative(destOperand: Expression, srcOperand: Int32) =
        Write {DestOperand=destOperand; SourceOperand=srcOperand; Native=true}

    let jumpIf(destination: Expression, equals: Boolean, less: Boolean) =
        JumpIf {Destination=destination; JumpIfEquals=equals; JumpIfLess=less}

    let jump(destination: Expression) =
        Jump {Destination=destination}

    let alloca(space: Int32) =
        Alloca(space)

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

