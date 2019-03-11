namespace Sacara.EndToEndTests

open System

module RuntimeTests =    

    let ``Test INC instruction``() =
        assert(Utility.executeScript("test_inc.sacara") = 42)

    let ``Test ADD instruction``() =
        assert(Utility.executeScript("test_add.sacara") = 96)

    let ``Test PUSH and POP instructions``() =
        assert(Utility.executeScript("test_push_and_pop.sacara") = 31337)

    let ``Test CALL instruction``() =
        assert(Utility.executeScript("test_call.sacara") = 13)

    let run() =
        ``Test INC instruction``()
        ``Test ADD instruction``()
        ``Test PUSH and POP instructions``()