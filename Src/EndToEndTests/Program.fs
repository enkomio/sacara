namespace Sacara.EndToEndTests

open System

module Program =    

    [<EntryPoint>]
    let main argv = 
        Console.WriteLine("-=[ Start Test ]=-")
        Console.WriteLine("If you want to test a specifiy Sacara DLL, pass it as argument to this program.")

        // instructions assembling
        AssemblerTests.run()

        // vm execution tests
        RuntimeTests.run()
        0
