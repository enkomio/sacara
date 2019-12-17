### 2.4 - 17/12/2019
* Implemented mod instruction to compute module operation
* Assembler now print warning message for not well written functions
* Added .mod directive
* .read, .write, .nread, .nwrite now support complex expression as .mov directive
* Added more test scripts
* Fixed minor bugs

### 2.3.0 - 24/11/2019
* Added directives: .jump, .add, .sub, .mul, .div, .cmp, .and, .or, .shiftl, .shiftr, .xor, .nor, .inc, .read.b, .read.w, .read.dw, .write.b, .write.w, .write.dw, .nread.b, .nread.w, .nread.dw, .nwrite.b, .nwrite.w, .nwrite.dw, .ncall
* Added directive to invoke methods in a more user friendly way
* Improved proc definition syntax in order to specify the parameters
* Added .mov directive to specify local var with a more powerful expression evaluation
* Added "include" statement to include an external script
* Added single line comment via "//"
* Now the arguments to main can be specified via Run method.
* read instruction now accepts an additional argument that specify the type to read (1 = byte, 2 = word, 3 = dword)
* write instruction now accepts an additional argument that specify the type to write (1 = byte, 2 = word, 3 = dword)
* nread instruction now accepts an additional argument that specify the type to read (1 = byte, 2 = word, 3 = dword)
* nwrite instruction now accepts an additional argument that specify the type to write (1 = byte, 2 = word, 3 = dword)
* It is now possible to specify the low level offset for local variable
* remove flag to check for stack cleaning in ncall instruction
* Fixed bugs and improved test suite

### 2.2.0 - 19/01/2019
* Implemented virtual instruction INC
* Implemented the .NET binding for vm_set_error_handler
* Made the code more C# friendly (see the C# example DotNetBindingWithErrorHandler)
* Refactored and cleaned code

### 2.1.0 - 11/11/2018
* Implemented routine to handle error during code execution.
* Fixed bug in .NET binding in order to ensure the context is always initialized
* Added example on Assembly loading encrypted with a Sacara script (part of blog post)

### 2.0.0 - 12/10/2018
* Added NOR usage obfuscation (thx to Solar Designer for the suggestion, based on his 1996 work hackme.com :O)
* Improved build script in order to select which compilation feature to use
* Some code improvmente/refactoring
* Added support for code creation via API interface (see example AssembleManagedInstructions)
* Created .NET binding to use Sacara from .NET (see example DotNetBinding)
* VM context now is an opaque value
* Added NOR instruction
* Added SETIP instruction
* Added SETSP instruction
* Added MASM samples

### 1.0.0 - 16/09/2018
* First Release.
