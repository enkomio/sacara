namespace ES.Sacara.Ir.Parser

open System

module IrAst =

    type Expression =
        | Number of Int32
        | Identifier of String

    and Statement =
        | Push of PushType
        | Pop of PopType
        | Procedure of ProcedureType
        | Label of LabelType
        | Call of CallType
        | JumpIf of JumpIfType
        | Jump of JumpType
        | Read of ReadType
        | Write of WriteType
        | Alloca of Int32
        | Byte of Int32
        | Word of Int32
        | DoubleWord of Int32
        | Ret
        | Nop
        | GetIp
        | Add
        | Empty

    and Program = 
        | Program of Statement list  
        
    and PushType = {
        Operand: Expression
    }

    and CallType = {
        Operand: Expression
        Native: Boolean
    }

    and JumpType = {
        Destination: Expression
    }

    and JumpIfType = {
        Destination: Expression
        JumpIfEquals: Boolean
        JumpIfLess: Boolean
    }

    and ReadType = {
        Operand: Expression
        Native: Boolean
    }

    and WriteType = {
        DestOperand: Expression
        SourceOperand: Int32
        Native: Boolean
    }

    and PopType = {
        Identifier: String
    }

    and ProcedureType = {
        Name: String
        Body: Statement list
    }

    and LabelType = {
        Name: String
        Statement: Statement
    }