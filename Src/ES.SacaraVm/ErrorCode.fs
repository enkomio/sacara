namespace ES.SacaraVm

open System

type ErrorCode =
    | VmHandlerNotFound
    | LocalVarOverflow

    static member Parse(errorCode: UInt32) =
        match errorCode with
        | 0x0FF000001u -> VmHandlerNotFound
        | 0x0FF000002u -> LocalVarOverflow
        | _ -> failwith "Unrecognized error Code"