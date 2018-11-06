namespace ES.SacaraVm

open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices

module internal NativeMethods =
    [<DllImport("kernel32.dll")>]
    extern IntPtr LoadLibrary(String dllToLoad)

    [<DllImport("kernel32.dll")>]
    extern IntPtr GetProcAddress(IntPtr hModule, String procedureName)

    [<DllImport("kernel32.dll")>]
    extern Boolean FreeLibrary(IntPtr hModule)

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmInitFunc = delegate of Byte array * UInt32 -> UInt32

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmRunFunc = delegate of UInt32 -> UInt32

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmFreeFunc = delegate of UInt32 -> unit

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmLocalVarSetFunc = delegate of UInt32 * UInt32 * UInt32 -> unit

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmLocalVarGetFunc = delegate of UInt32 * UInt32 -> UInt32

    let getFunc<'T>(handle: IntPtr, funcName: String) =
        let funcAddress = GetProcAddress(handle, funcName)
        Marshal.GetDelegateForFunctionPointer(funcAddress, typeof<'T>) :> Object :?> 'T
    
type SacaraVm() = 
    let mutable _handle = IntPtr.Zero
    let mutable _vmContext = uint32 0
    let mutable _vmInit : NativeMethods.VmInitFunc option = None
    let mutable _vmFree : NativeMethods.VmFreeFunc option = None
    let mutable _vmRun : NativeMethods.VmRunFunc option = None
    let mutable _vmLocalVarSet : NativeMethods.VmLocalVarSetFunc option = None
    let mutable _vmLocalVarGet : NativeMethods.VmLocalVarGetFunc option = None
    let mutable _localVars = List.empty<UInt32 * UInt32>
    
    do
        let sacaraVmDll = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SacaraVm.dll")
        if not(File.Exists(sacaraVmDll)) then
            failwith(String.Format("Unable to find the sacara DLL: {0}", sacaraVmDll))

        _handle <- NativeMethods.LoadLibrary(sacaraVmDll)
        _vmInit <- Some <| NativeMethods.getFunc<NativeMethods.VmInitFunc>(_handle, "vm_init")
        _vmRun <- Some <| NativeMethods.getFunc<NativeMethods.VmRunFunc>(_handle, "vm_run")
        _vmFree <- Some <| NativeMethods.getFunc<NativeMethods.VmFreeFunc>(_handle, "vm_free")
        _vmLocalVarSet <- Some <| NativeMethods.getFunc<NativeMethods.VmLocalVarSetFunc>(_handle, "vm_local_var_set")
        _vmLocalVarGet <- Some <| NativeMethods.getFunc<NativeMethods.VmLocalVarGetFunc>(_handle, "vm_local_var_get")

    member this.Run(code: Byte array) =
        _vmContext <- _vmInit.Value.Invoke(code, uint32 code.Length)
        _localVars |> List.iter(fun (index, value) -> _vmLocalVarSet.Value.Invoke(_vmContext, index, value))
        _vmRun.Value.Invoke(_vmContext) |> ignore

    member this.LocalVarSet(index: Int32, value: Int32) =
        _localVars <- (uint32 index, uint32 value)::_localVars        

    member this.LocalVarGet(index: Int32) =
        _vmLocalVarGet.Value.Invoke(_vmContext, uint32 index)

    member this.Free() =
        _vmFree.Value.Invoke(_vmContext)
        _vmContext <- uint32 0
        
    member this.Dispose() =  
        if int32 _vmContext <> 0 then 
            // release the Vm context if the Free wasn't invoked
            this.Free()
        NativeMethods.FreeLibrary(_handle) |> ignore

    interface IDisposable with
        member this.Dispose() =
            this.Dispose()

