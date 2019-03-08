namespace Sacara.EndToEndTests

open System

module Program =
    [<EntryPoint>]
    let main argv = 
        AssemblerTests.``Assemble text - default settings``()
        0
