namespace ES.Sacara.Ir.Parser

open System
open Microsoft.FSharp.Text.Lexing
open ES.Sacara.Ir.Parser.SacaraIrLexer
open ES.Sacara.Ir.Parser.SacaraIrParser

exception ParsingException of String
exception TokenizationExcpetion of String

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

    let tokenize(irCode: String, lexbuf: LexBuffer<_>) =
        try
            // tokenize the code
            _tokens <- tokenizeLexBuffer(lexbuf)
        with e ->
            let msg = String.Format("Error during tokenization at position: {0},{1}", lexbuf.StartPos.Line, lexbuf.StartPos.Column)
            raise(TokenizationExcpetion msg)

    let parse(lexbuf: LexBuffer<_>) = 
        try
            SacaraIrParser.program tokenProvider lexbuf
        with e ->
            let curToken = _tokens.[_tokenIndex]
            let msg = String.Format("Error during parsing at position: {0},{1}. Unexpected input token: {2}", curToken.StartPos.Line, curToken.StartPos.Column, curToken.Token)
            raise(ParsingException msg)
        
    member this.Parse(irCode: String) =        
        // init lexbuf
        let lexbuf = LexBuffer<_>.FromString irCode
        let startPos = lexbuf.StartPos
        let endPos = lexbuf.EndPos

        tokenize(irCode, lexbuf)

        // reset the lexer
        lexbuf.StartPos <- startPos
        lexbuf.EndPos <- endPos
        lexbuf.IsPastEndOfStream <- false

        parse(lexbuf)