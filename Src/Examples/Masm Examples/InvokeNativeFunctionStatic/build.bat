\MASM32\BIN\RC.EXE /v InvokeNativeFunctionStatic.rc
\MASM32\BIN\ML.EXE /c /coff /Cp /nologo /I"\MASM32\INCLUDE" InvokeNativeFunctionStatic.asm
\MASM32\BIN\LINK.EXE /SUBSYSTEM:WINDOWS /RELEASE /VERSION:4.0 /LIBPATH:"\MASM32\LIB" /OUT:"InvokeNativeFunctionStatic.exe" InvokeNativeFunctionStatic.obj InvokeNativeFunctionStatic.res