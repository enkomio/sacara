namespace ES.Sacara.Ir.Assembler

open System

type AssemblerSettings() =
    member val InserJunkOpCodes = false with get, set
    member val ReorderFunctions = false with get, set
    member val UseMultipleOpcodeForSameInstruction = false with get, set
    member val RandomlyEncryptOpCode = false with get, set