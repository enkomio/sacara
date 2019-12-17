namespace Sacara.EndToEndTests

open System
open System.Text
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions
open System.Reflection
open System.Runtime.InteropServices
        
module RuntimeTests =
    let private _executedScripts = new HashSet<String>()

    [<DllImport("kernel32.dll", SetLastError = true)>]
    extern void VirtualProtect(IntPtr lpAddress, UInt32 dwSize, UInt32 flNewProtect, IntPtr lpflOldProtect)
    
    let private checkExecuteScriptWithArg(scriptFilename: String, args: Int32 array) =
        _executedScripts.Add(scriptFilename) |> ignore
        executeScriptWithArg(scriptFilename, args)

    let private runScriptWithArgs(scriptFile: String, args: Int32 array) =
        let scriptContent = File.ReadAllText(scriptFile).Trim()
        let m = Regex.Match(scriptContent, "// result: ([0-9]+)")
        if m.Success then
            let expectedValue = Int32.Parse(m.Groups.[1].Value)
            let scriptFilename = Path.GetFileName(scriptFile)
            assertTest(checkExecuteScriptWithArg(scriptFilename, args), expectedValue)

    let private runScript(scriptFile: String) =
        runScriptWithArgs(scriptFile, Array.empty)
        
    let private runLocalScriptWithArgs(scriptFile: String, args: Int32 array) =
        runScriptWithArgs(getScriptFullPath(scriptFile), args)

    let private runLocalScript(scriptFile: String) =
        runScriptWithArgs(getScriptFullPath(scriptFile), Array.empty)

    let private verifyAllTestsScriptWereExecuted() =
        let scriptDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "TestSources")
        let totalScripts = Directory.GetFiles(scriptDir, "test_*.sacara", SearchOption.AllDirectories)
        if totalScripts.Length <> _executedScripts.Count then
            let executedScripts = _executedScripts |> Seq.map(Path.GetFileName) |> Set.ofSeq
            let createdScripts = totalScripts |> Seq.map(Path.GetFileName) |> Set.ofSeq
            let commondScripts = Set.difference createdScripts executedScripts
            Console.WriteLine("Script not executed during tests")
            commondScripts |> Seq.iter(Console.WriteLine)
            failwith "Not all tests script were executed"
            
    let ``Test XOR decode a buffer``() =
        let buffer = Encoding.UTF8.GetBytes("this is the text that I'll use to verify if it correctly decrypted during the execution")
        let key = Encoding.UTF8.GetBytes("password for test")
        let inputBuffer = buffer |> Array.mapi(fun i b -> b ^^^ key.[i % key.Length])
        let resultBuffer = Array.zeroCreate<Byte>(inputBuffer.Length)

        // compute args
        let memBuffer = Marshal.AllocHGlobal(inputBuffer.Length)        
        Marshal.Copy(inputBuffer, 0, memBuffer, inputBuffer.Length)
        let args = [|memBuffer.ToInt32(); inputBuffer.Length|]

        // run the test
        checkExecuteScriptWithArg("test_decode_xor_buffer.sacara", args) |> ignore
        Marshal.Copy(memBuffer, resultBuffer, 0, resultBuffer.Length)
        Marshal.FreeHGlobal(memBuffer)
        
        let resultBufferString = Encoding.UTF8.GetString(resultBuffer)
        let bufferString = Encoding.UTF8.GetString(buffer)

        assertTest(resultBufferString.CompareTo(bufferString), 0)
        
    let ``Test main proc with explicit parameters``() =
        assertTest(checkExecuteScriptWithArg("test_main_proc_with_explicit_parameters.sacara", [|24500; 175; 2|]), 0xC001) 
                
    let ``Test NCALL instruction with CDECL calling convention``() =
        let code = [|
            0x55uy;                     // push ebp       
            0x8Buy; 0xECuy;             // mov ebp,esp    
            0x33uy; 0xD2uy;             // xor edx, edx
            0x8Buy; 0x5Duy; 0x0Cuy      // mov ebx, dword ptr [ebp+Ch] 
            0x8Buy; 0x45uy; 0x08uy      // mov eax, dword ptr [ebp+8h]  
            0xF7uy; 0xF3uy              // div eax, ebx
            0x8Buy; 0xE5uy;             // mov esp, ebp  
            0x5Duy;                     // pop ebp  
            0xc3uy                      // ret
        |]

        let oldProtection = GCHandle.Alloc(0, GCHandleType.Pinned).AddrOfPinnedObject()                
        let codeMem = GCHandle.Alloc(code, GCHandleType.Pinned)
        let codePtr = codeMem.AddrOfPinnedObject()        
        VirtualProtect(codePtr, uint32 code.Length, uint32 0x40, oldProtection)
        runLocalScriptWithArgs("test_ncall.sacara", [|codePtr.ToInt32(); 27; 3|])

    let ``Test NCALL instruction with STDECL calling convention``() =
        let code = [|
            0x55uy;                     // push ebp       
            0x8Buy; 0xECuy;             // mov ebp,esp    
            0x33uy; 0xD2uy;             // xor edx, edx
            0x8Buy; 0x5Duy; 0x0Cuy      // mov ebx, dword ptr [ebp+Ch] 
            0x8Buy; 0x45uy; 0x08uy      // mov eax, dword ptr [ebp+8h]  
            0xF7uy; 0xF3uy              // div eax, ebx
            0x8Buy; 0xE5uy;             // mov esp, ebp  
            0x5Duy;                     // pop ebp  
            0xC2uy; 0x04uy; 0x00uy      // ret 4
        |]

        let oldProtection = GCHandle.Alloc(0, GCHandleType.Pinned).AddrOfPinnedObject()                
        let codeMem = GCHandle.Alloc(code, GCHandleType.Pinned)
        let codePtr = codeMem.AddrOfPinnedObject()        
        VirtualProtect(codePtr, uint32 code.Length, uint32 0x40, oldProtection)
        runLocalScriptWithArgs("test_ncall.sacara", [|codePtr.ToInt32(); 81; 9|])        
                
    let ``Test NREAD instruction``() =
        let mem = Marshal.AllocHGlobal(8)        
        Marshal.WriteInt64(mem, 0x0043434343424241L)
        runLocalScriptWithArgs("test_nread.sacara", [|mem.ToInt32()|])
        Marshal.FreeHGlobal(mem)
        
    let ``Test NWRITE instruction``() =
        let mem = Marshal.AllocHGlobal(8)
        Marshal.WriteInt64(mem, 0x00L)
        runLocalScriptWithArgs("test_nwrite.sacara", [|mem.ToInt32()|])
        Marshal.FreeHGlobal(mem)

    let private runSelfContainedTests() =
        let selfContainedScript = Path.Combine(scriptDir, "SelfContained")
        Directory.GetFiles(selfContainedScript, "test_*.sacara")
        |> Array.iter(runScript)
                
    let run() =      
        _executedScripts.Clear()
        runSelfContainedTests()
        runTests("RuntimeTests")
        verifyAllTestsScriptWereExecuted()