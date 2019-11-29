namespace ES.Utility

open System
open System.IO
open Argu
open ES.Fslog
open ES.Fslog.Loggers
open ES.Fslog.TextFormatters
open System.Runtime.InteropServices

[<AutoOpen>]
module NativeMethods =
    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern IntPtr BeginUpdateResource(String pFileName, [<MarshalAs(UnmanagedType.Bool)>]Boolean bDeleteExistingResources);

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern Boolean UpdateResource(IntPtr hUpdate, string lpType, String lpName, UInt16 wLanguage, IntPtr lpData, UInt32 cbData);

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern Boolean EndUpdateResource(IntPtr hUpdate, Boolean fDiscard);

[<AutoOpen>]
module Cli =
    type CLIArguments =
        | [<MainCommand; Last>] Filename of filename:String
        | [<Mandatory>] Resource of resource:String
        | [<AltCommandLineAttribute("-n")>] Name of name:String
        | Verbose
    with
        interface IArgParserTemplate with
            member s.Usage =
                match s with
                | Filename _ -> "the Filename to modify."
                | Resource _ -> "the file to add as resource."
                | Name _ -> "the name of the resource."
                | Verbose -> "print verbose messages."

module Program =
    let private _logger =
        log "Program"
        |> info "ModifyFile" "Modify file: {0}"
        |> info "ResourceFile" "Resource file: {0}"
        |> info "Completed" "Resource added"
        |> error "ResourceNotAdded" "Unable to add the resource"
        |> error "FileNotFound" "File '{0}' not found"
        |> error "ResourceFileNotFound" "Resource file '{0}' not found"
        |> build

    let private printColor(msg: String, color: ConsoleColor) =
        Console.ForegroundColor <- color
        Console.WriteLine(msg)
        Console.ResetColor() 

    let private printError(errorMsg: String) =
        printColor(errorMsg, ConsoleColor.Red)

    let private printBanner() =
        Console.ForegroundColor <- ConsoleColor.Cyan        
        let banner = "-=[ Sacara VM - Add PE Resource ]=-"
        let year = if DateTime.Now.Year = 2019 then "2019" else String.Format("2019-{0}", DateTime.Now.Year)
        let copy = String.Format("Copyright (c) {0} Enkomio {1}", year, Environment.NewLine)
        Console.WriteLine(banner)
        Console.WriteLine(copy)
        Console.ResetColor()

    let private printUsage(body: String) =
        Console.WriteLine(body)

    let private configureLogger(isVerbose: Boolean) =
        let logProvider = new LogProvider()    
        let logLevel = if isVerbose then LogLevel.Verbose else LogLevel.Informational
        logProvider.AddLogger(new ConsoleLogger(logLevel, new ConsoleLogFormatter()))
        logProvider.AddLogSourceToLoggers(_logger)

    let private addResource(peFilename: String, resource: Byte array, resourceName: String) =
        let buffer = GCHandle.Alloc(resource, GCHandleType.Pinned);

        // update the resource                
        let handle = BeginUpdateResource(peFilename, true)
        if UpdateResource(handle, "RT_RCDATA", resourceName, uint16 0, buffer.AddrOfPinnedObject(), Convert.ToUInt32(resource.Length)) then
            EndUpdateResource(handle, false)
        else
            false

    [<EntryPoint>]
    let main argv = 
        printBanner()
        
        let parser = ArgumentParser.Create<CLIArguments>()
        try            
            let results = parser.Parse(argv)
                    
            if results.IsUsageRequested then
                printUsage(parser.PrintUsage())
                0
            else
                let isVerbose = results.Contains(<@ Verbose @>)
                configureLogger(isVerbose)

                let fileToModify = results.GetResult(<@ Filename @>)
                let resourceToAdd = results.GetResult(<@ Resource @>)
                let resourceName = results.GetResult(<@ Name @>, "RES0")        
            
                if not <| File.Exists(fileToModify) then
                    _logger?FileNotFound(fileToModify)
                elif not <| File.Exists(resourceToAdd) then
                    _logger?ResourceFileNotFound(resourceToAdd)
                else
                    _logger?ModifyFile(fileToModify)
                    _logger?ResourceFile(resourceToAdd)

                    let resource = File.ReadAllBytes(resourceToAdd)

                    if addResource(fileToModify, resource, resourceName) 
                    then _logger?Completed() 
                    else _logger?ResourceNotAdded()
                0
        with 
            | :? ArguParseException ->
                printUsage(parser.PrintUsage())   
                1
            | e ->
                printError(e.ToString())
                1
