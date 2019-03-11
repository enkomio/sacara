namespace Sacara.EndToEndTests

open System
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type MockType() =
    static member AddNumbers(_: UInt32, _: UInt32, a: UInt32, b: UInt32) =
        // no idea what are the first two parameters, need to dig deeper
        a + b

module RuntimeTests =
    let ``Test INC instruction``() =
        assert(Utility.executeScript("test_inc.sacara") = 42)

    let ``Test ADD instruction``() =
        assert(Utility.executeScript("test_add.sacara") = 96)

    let ``Test PUSH and POP instructions``() =
        assert(Utility.executeScript("test_push_and_pop.sacara") = 31337)

    let ``Test CALL instruction``() =
        assert(Utility.executeScript("test_call.sacara") = 13)

    let ``Test NCALL instruction``() =
        let addNumbersMethod = typeof<MockType>.GetMethod("AddNumbers")
        let methodHandle = addNumbersMethod.MethodHandle
        let functionPointer = methodHandle.GetFunctionPointer().ToInt32()

        assert(Utility.executeScriptWithArg("test_ncall.sacara", [|functionPointer|]) = 100)

    let run() =
        ``Test INC instruction``()
        ``Test ADD instruction``()
        ``Test PUSH and POP instructions``()
        ``Test NCALL instruction``()
