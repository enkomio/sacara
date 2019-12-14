namespace ES.Sacara.Ir.Parser

open System
open Microsoft.FSharp.Text.Lexing
open System.Text.RegularExpressions
open ES.Sacara.Ir.Parser.SacaraIrParser
open ES.Sacara.Ir.Core

[<AutoOpen>]
module internal IrLexerUtility =

    let private incrementLine(lexbuf:LexBuffer<_>) =
        lexbuf.StartPos <- lexbuf.StartPos.NextLine
        lexbuf.EndPos <- lexbuf.EndPos.NextLine

    let directives =
        [
            (".jump", JUMP_DIRECTIVE)
            (".mov", MOV_DIRECTIVE)
            (".add", ADD_DIRECTIVE)
            (".sub", SUB_DIRECTIVE)
            (".mul", MUL_DIRECTIVE)
            (".div", DIV_DIRECTIVE)
            (".cmp", CMP_DIRECTIVE)
            (".and", AND_DIRECTIVE)
            (".or", OR_DIRECTIVE)
            (".shiftr", SHIFTR_DIRECTIVE)
            (".shiftl", SHIFTL_DIRECTIVE)
            (".xor", XOR_DIRECTIVE)
            (".nor", NOR_DIRECTIVE)
            (".inc", INC_DIRECTIVE)
            (".read.b", READB_DIRECTIVE)
            (".read.w", READW_DIRECTIVE)
            (".read.dw", READDW_DIRECTIVE)
            (".write.b", WRITEB_DIRECTIVE)
            (".write.w", WRITEW_DIRECTIVE)
            (".write.dw", WRITEDW_DIRECTIVE)
            (".nread.b", NREADB_DIRECTIVE)
            (".nread.w", NREADW_DIRECTIVE)
            (".nread.dw", NREADDW_DIRECTIVE)
            (".nwrite.b", NWRITEB_DIRECTIVE)
            (".nwrite.w", NWRITEW_DIRECTIVE)
            (".nwrite.dw", NWRITEDW_DIRECTIVE)
            (".ncall", NCALL_DIRECTIVE)
        ]
        |> Map.ofList

    let globalStatements =
        [
            ("include", INCLUDE)
        ] 
        |> Map.ofList

    let keywords = 
        [
            (IrInstruction.Push, PUSH)
            (IrInstruction.Pop, POP)
            (IrInstruction.Add, ADD)
            (IrInstruction.Call, CALL)
            (IrInstruction.NativeCall, NCALL)
            (IrInstruction.Write, WRITE)
            (IrInstruction.NativeWrite, NWRITE)
            (IrInstruction.Read, READ)
            (IrInstruction.NativeRead, NREAD)            
            (IrInstruction.Nop, NOP)
            (IrInstruction.GetIp, GETIP)
            (IrInstruction.Ret, RET)
            (IrInstruction.Jump, JUMP)
            (IrInstruction.JumpIfLess, JUMPIFL)
            (IrInstruction.JumpIfLessEquals, JUMPIFLE)
            (IrInstruction.JumpIfGreater, JUMPIFG)
            (IrInstruction.JumpIfGreaterEquals, JUMPIFGE)
            (IrInstruction.Alloca, ALLOCA)
            (IrInstruction.Byte, BYTE)
            (IrInstruction.Word, WORD)
            (IrInstruction.DoubleWord, DWORD)
            (IrInstruction.Halt, HALT)
            (IrInstruction.Cmp, CMP)
            (IrInstruction.GetSp, GETSP)
            (IrInstruction.StackWrite, SWRITE)
            (IrInstruction.StackRead, SREAD)
            (IrInstruction.Sub, SUB)
            (IrInstruction.Mul, MUL)
            (IrInstruction.Div, DIV)
            (IrInstruction.And, AND)
            (IrInstruction.ShiftLeft, SHIFTL)
            (IrInstruction.ShiftRight, SHIFTR)
            (IrInstruction.Or, OR)
            (IrInstruction.Not, NOT)
            (IrInstruction.Xor, XOR)
            (IrInstruction.Nor, NOR)
            (IrInstruction.SetIp, SETIP)
            (IrInstruction.SetSp, SETSP)            
            (IrInstruction.Inc, INC)
            (IrInstruction.Mod, MOD)
        ] 
        |> List.map(fun (irInstruction, token) -> (string irInstruction, token))
        |> fun bindingList -> ([
            // these instruction doesn't have a corrispetive in the instructions definition            
            ("proc", PROC)
            ("endp", ENDP)
        ] @ bindingList)
        |> Map.ofList

    let getString (lexbuf : LexBuffer<_>) = 
        LexBuffer<_>.LexemeString lexbuf

    let getHexString (lexbuf : LexBuffer<_>) = 
        let str = getString lexbuf
        let num = Convert.ToInt32(str.Substring(2), 16)
        Char.ConvertFromUtf32(num)

    let noToken(lexbuf: LexBuffer<_>) =
        lexbuf

    let newToken(token: token) (lexbuf: LexBuffer<_>) =
        token

    let nextLine (lexbuf:LexBuffer<_>) =        
        incrementLine(lexbuf)
        lexbuf

    let label(lexbuf: LexBuffer<_>) =
        let identifier = Regex.Unescape(getString lexbuf)
        newToken (LABEL identifier) lexbuf

    let directive(lexbuf: LexBuffer<_>) =
        let directive = Regex.Unescape(getString lexbuf)
        match directives |> Map.tryFind directive with
        | Some directiveToken -> newToken directiveToken lexbuf
        | None -> newToken (INVOKE_DIRECTIVE directive) lexbuf

    let identifier(lexbuf: LexBuffer<_>) =
        let identifier = Regex.Unescape(getString lexbuf).ToLower()
        match keywords |> Map.tryFind identifier with
        | Some identifierToken -> newToken identifierToken lexbuf
        | None -> newToken (IDENTIFIER identifier) lexbuf

    let globalStatement(lexbuf: LexBuffer<_>) =
        let keyword = Regex.Unescape(getString lexbuf).ToLower()
        let keywordToken = globalStatements |> Map.find keyword
        newToken keywordToken lexbuf

    let integer(lexbuf: LexBuffer<_>) =
        let number = Regex.Unescape(getString lexbuf)
        newToken (INTEGER(int32(number))) lexbuf