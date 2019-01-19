namespace ES.SacaraVm

open System
open System.IO
open System.Reflection
open ES.Sacara.Ir.Assembler
    
type SacaraVm(sacaraVmDll: String) = 
    let mutable _handle = IntPtr.Zero
    let mutable _vmContext = uint32 0
    let mutable _vmInit : NativeMethods.VmInitFunc option = None
    let mutable _vmFree : NativeMethods.VmFreeFunc option = None
    let mutable _vmRun : NativeMethods.VmRunFunc option = None
    let mutable _vmLocalVarSet : NativeMethods.VmLocalVarSetFunc option = None
    let mutable _vmLocalVarGet : NativeMethods.VmLocalVarGetFunc option = None
    let mutable _vmSetErrorHandler : NativeMethods.VmSetErrorHandler option = None

    let mutable _localVars = List.empty<UInt32 * UInt32>
    let mutable _vmErrorHandlerCallback: Action<UInt32, UInt32> option = None

    do        
        if not(File.Exists(sacaraVmDll)) then
            failwith(String.Format("Unable to find the sacara DLL: {0}", sacaraVmDll))

        _handle <- NativeMethods.LoadLibrary(sacaraVmDll)
        _vmInit <- Some <| NativeMethods.getFunc<NativeMethods.VmInitFunc>(_handle, "vm_init")
        _vmRun <- Some <| NativeMethods.getFunc<NativeMethods.VmRunFunc>(_handle, "vm_run")
        _vmFree <- Some <| NativeMethods.getFunc<NativeMethods.VmFreeFunc>(_handle, "vm_free")
        _vmLocalVarSet <- Some <| NativeMethods.getFunc<NativeMethods.VmLocalVarSetFunc>(_handle, "vm_local_var_set")
        _vmLocalVarGet <- Some <| NativeMethods.getFunc<NativeMethods.VmLocalVarGetFunc>(_handle, "vm_local_var_get")
        _vmSetErrorHandler <- Some <| NativeMethods.getFunc<NativeMethods.VmSetErrorHandler>(_handle, "vm_set_error_handler")

    new() = new SacaraVm(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SacaraVm.dll"))

    member this.Run(code: Byte array) =
        _vmContext <- _vmInit.Value.Invoke(code, uint32 code.Length)
        _localVars |> List.iter(fun (index, value) -> _vmLocalVarSet.Value.Invoke(_vmContext, index, value))

        if _vmErrorHandlerCallback.IsSome then
            let effectiveCallback(ip: UInt32) (errorCode: UInt32) = _vmErrorHandlerCallback.Value.Invoke(ip, errorCode)
            _vmSetErrorHandler.Value.Invoke(_vmContext, new NativeMethods.ErrorHandlerCallback(effectiveCallback))

        _vmRun.Value.Invoke(_vmContext) |> ignore

    member this.Run(code: IrAssemblyCode) =
        this.Run(code.GetBuffer())

    member this.LocalVarSet(index: Int32, value: Int32) =
        _localVars <- (uint32 index, uint32 value)::_localVars        

    member this.LocalVarGet(index: Int32) =
        _vmLocalVarGet.Value.Invoke(_vmContext, uint32 index)

    member this.SetErrorHandler(callback: Action<UInt32, UInt32>) =
        _vmErrorHandlerCallback <- Some callback

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

