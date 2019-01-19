namespace ES.SacaraVm

open System
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

    [<UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)>]    
    type ErrorHandlerCallback = delegate of UInt32 * UInt32 -> unit

    [<UnmanagedFunctionPointer(CallingConvention.StdCall)>]
    type VmSetErrorHandler = delegate of UInt32 * ErrorHandlerCallback -> unit

    let getFunc<'T>(handle: IntPtr, funcName: String) =
        let funcAddress = GetProcAddress(handle, funcName)
        Marshal.GetDelegateForFunctionPointer(funcAddress, typeof<'T>) :> Object :?> 'T