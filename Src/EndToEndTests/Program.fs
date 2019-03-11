namespace Sacara.EndToEndTests

open System

module Program =    

    [<EntryPoint>]
    let main argv = 
        // instructions assembling
        AssemblerTests.run()
        0
