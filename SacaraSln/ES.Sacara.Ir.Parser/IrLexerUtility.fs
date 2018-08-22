namespace ES.Sacara.Ir.Parser

open System
open Microsoft.FSharp.Text.Lexing
open System.Text.RegularExpressions
open ES.Sacara.Ir.Parser.SacaraIrParser

[<AutoOpen>]
module internal IrLexerUtility =    

    let private incrementLine(lexbuf:LexBuffer<_>) =
        lexbuf.StartPos <- lexbuf.StartPos.NextLine
        lexbuf.EndPos <- lexbuf.EndPos.NextLine

    let keywords = 
        [
            ("push", PUSH)
            ("pop", POP)
            ("add", ADD)
            ("call", CALL)
            ("ncall", NCALL)
            ("write", WRITE)
            ("nwrite", NWRITE)
            ("read", READ)
            ("nread", NREAD)
            ("proc", PROC)
            ("endp", ENDP)
            ("nop", NOP)
            ("getip", GETIP)
            ("ret", RET)
            ("jump", JUMP)
            ("jumpifl", JUMPIFL)
            ("jumpifle", JUMPIFLE)
            ("jumpifg", JUMPIFG)
            ("jumpifge", JUMPIFGE)
            ("alloca", ALLOCA)
            ("byte", BYTE)
            ("word", WORD)
            ("dword", DWORD)
        ] |> Map.ofList

    let getString (lexbuf : LexBuffer<_>) = 
        LexBuffer<_>.LexemeString lexbuf

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

    let identifier(lexbuf: LexBuffer<_>) =
        let identifier = Regex.Unescape(getString lexbuf)
        match keywords |> Map.tryFind identifier with
        | Some identifierToken -> newToken identifierToken lexbuf
        | None -> newToken (IDENTIFIER identifier) lexbuf

    let integer(lexbuf: LexBuffer<_>) =
        let number = Regex.Unescape(getString lexbuf)
        newToken (INTEGER(int32(number))) lexbuf