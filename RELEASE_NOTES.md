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
