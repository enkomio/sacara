namespace ES.Sacara.Ir.Parser

open System
open Microsoft.FSharp.Text.Lexing
open ES.Sacara.Ir.Parser.SacaraIrLexer
open ES.Sacara.Ir.Parser.SacaraIrParser

type internal TokenInfo = {
    Token: token
    StartPos: Position
    EndPos: Position
}  with
    member this.IsNewLine() =
        this.Token = NEWLINE

type SacaraAstBuilder() =
    // some fields for error detection
    let mutable _tokens = List.empty<TokenInfo>    
    let mutable _tokenIndex = 0

    let createTokenInfo(tkn: token, lexbuf: LexBuffer<_>) = {
        Token = tkn
        StartPos = lexbuf.StartPos
        EndPos = lexbuf.EndPos
    }
        

    let rec tokenizeLexBuffer(lexbuf: LexBuffer<_>) = [
        let nextToken = irCode lexbuf        
        
        if nextToken <> EOF then 
            yield createTokenInfo(nextToken, lexbuf)
            yield! tokenizeLexBuffer(lexbuf)
        else 
            yield createTokenInfo(nextToken, lexbuf)
    ]

    let tokenProvider(lexbuf: LexBuffer<_>) =
        while _tokens.[_tokenIndex].IsNewLine() do
            _tokenIndex <- _tokenIndex + 1

        let tokenInfo = _tokens.[_tokenIndex]
        _tokenIndex <- _tokenIndex + 1
        tokenInfo.Token
        
    member this.Parse(irCode: String) =
        try
            // init lexbuf
            let lexbuf = LexBuffer<_>.FromString irCode
            let startPos = lexbuf.StartPos
            let endPos = lexbuf.EndPos

            // tokenize the code
            _tokens <- tokenizeLexBuffer(lexbuf)

            // reset the lexer
            lexbuf.StartPos <- startPos
            lexbuf.EndPos <- endPos
            lexbuf.IsPastEndOfStream <- false

            SacaraIrParser.program tokenProvider lexbuf
        with e ->
            reraise()
