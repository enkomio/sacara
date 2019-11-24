namespace ES.Sacara.Ir.Parser

open System

module IrAst =

    type Expression =
        | Number of Int32
        | Identifier of String
        | StatementExpression of Statement

    and Statement =
        | IncludeFile of String
        | Push of PushType
        | Pop of PopType
        | Inc
        | Procedure of ProcedureType
        | Label of LabelType
        | Call of CallType
        | JumpIf of JumpIfType        
        | Read of ReadType
        | Write of WriteType
        | Alloca
        | Jump
        | Ret
        | Nop
        | GetIp
        | Add
        | Empty
        | Halt
        | Cmp
        | GetSp
        | StackWrite
        | StackRead
        | Sub
        | Mul
        | Div
        | And
        | Or
        | Not
        | Xor
        | Nor
        | ShiftRight
        | ShiftLeft
        | SetIp
        | SetSp
        | Byte of Int32 list
        | Word of Int32 list
        | DoubleWord of Int32 list
        | Block of Statement list  
        
    and Program = 
        | Program of Statement list  
        
    and PushType = {
        Operand: Expression
    }

    and CallType = {
        Native: Boolean
    }

    and JumpIfType = {
        JumpIfEquals: Boolean
        JumpIfLess: Boolean
    }

    and ReadType = {
        Native: Boolean
    }

    and WriteType = {
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