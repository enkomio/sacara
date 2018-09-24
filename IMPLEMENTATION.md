# Architecture

_Sacara_ is implemented by considering a stack based VM, this means that most of the instructions take their arguments from the stack. It also support local variables that are saved in a different memory area. Both local variables and stack frame are valid only in the current function execution context. Once that you return from a function their values is lost.

Find below a diagram of the _Sacara_ architecture:

<img src="https://raw.githubusercontent.com/enkomio/media/master/sacara/sacara_architecture.png"/>

All values stored inside the stack and local variables are considered as *DOUBLE WORD* in size. However, some instruction (as *read* or *write*) will convert the DOUBLE WORD value to a BYTE value.

The main entry point is a function named **main**.

### Assembling a script
In order to assemble a *Sacara* script you have to use the **SacaraAsm** utility. The utility support various kind of obfuscation options, print the help to know more.

The Sacara source code is called **SIL** which stands for **Sacara Intermediate Language**.

### Exported VM methods
The *SacaraVM* DLL exports four methods that can be invoked programmatically. You can find an <a href="https://github.com/enkomio/sacara/blob/master/Src/Examples/InvokeNativeFunction/main.c#L42">example of usage in the <strong>Examples</strong> directory</a>.

# Functions, status flags, labels and comments
*Sacara* allows to define functions, to reference labels and to add comments to your code.

### Functions
A function is defined with the **proc** keyword followed by the function name. All functions must end with the **endp** keyword. Find below an example of definition:

```
proc main
    push 0x123
    halt
endp
```

### Label
You can define a **label** inside your code. In *Sacara* each *label* is absolute, this mean that you can reference a label defined in a function from an external function. This implies that all label names in your code must be unique. The function name is also considered a label, and you can reference it in the same exact way. Find below an example of label definition and referencing:

```
proc main
    push func1
    call
    halt
endp

proc func1
    jump my_label
    nop
my_label:
    push 0x123
    ret
endp
```

### Comments
You can insert comments in your code to make it more understandable. *Sacara* support multi lines comment which starts with the string **/*** and ends with the string ***/** (this is the same exact pattern used in the C programming language). Find below an example of comment usages:

```
proc main
    push 0x123   /* push second argument */
    push 0x456   /* push first argument */
    add          /* sum */
    halt         /* stop execution */
endp
```

# Instruction set
*Sacara* supports a good amount of instructions. Each instruction can accept 0 or more arguments. Most of the instructions (except for *push* and *pop*) will get the arguments from the stack. When we refer to the first argument, it is the first value that is popped from the stack. The following image show an example of stack layout after pushing the given arguments:

<img src="https://raw.githubusercontent.com/enkomio/media/master/sacara/sacara_stack.png" />

### RET
*Mnemonic*: **ret**

*Popped Arguments*: **0**

*Pushed Arguments*: **at most one**

This instruction returns from a function, if there is a value on top of the stack it is pushed in the caller stack. The function return address is saved on top of the caller stack. When the function returns all the stack and local variables are destroyed.  

### NOP
*Mnemonic*: **nop**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This instruction does nothing, just increase the Instruction Pointer.

### ADD
*Mnemonic*: **add**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction add two values and push back the result. It pops from the stack: 
* first operand
* second operand

### PUSH
*Mnemonic*: **push <imm>/<label>/<local variable>**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction accepts a parameter taken from the bytecode and push the value on top of the stack. It can be used with an immutable, with a label or with a local var. Find below an example of usage:

```
proc main
my_label:
    push my_label         /* push the offset of the label */
    push 0x123            /* push the immediate value 0x123 */
    push local_var        /* push the value stored in local_var */
    push my_func          /* push the offset of the my_func function */
endp

proc my_func
    ret
endp
```

### POP
*Mnemonic*: **pop <local variable>**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction pop from the stack a value and save the result into a local variable.

### CALL
*Mnemonic*: **call**

*Popped Arguments*: **at least 2**

*Pushed Arguments*: **0**

This instruction allows to call a user defined method. It pops from the stack: 
* the name of the method to call
* the number of argument to push in the new stack frame
* the arguments.

### NATIVE CALL
*Mnemonic*: **ncall**

*Popped Arguments*: **at least 3**

*Pushed Arguments*: **1**

This instruction allows to call a *native* method outside of the VM. It pops from the stack:
* the address of the method to call
* the number of argument to push in the stack
* the number of DOUBLE WORD to remove from the stack, after that the method returns
* the arguments to the method

When the native method returns, the value of the native *EAX* register is pushed on top of the stack.

### READ
*Mnemonic*: **read**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction allows to read 1 byte from the Sacara SIL code at a given offset (which start from 0) and push the result into the stack. It pops from the stack:
* the offset of the VM IP

### NATIVE READ
*Mnemonic*: **nread**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction allows to read 1 byte from the native memory space and push the result into the stack. It pops from the stack:
* the native address to read

### WRITE
*Mnemonic*: **write**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction allows to write 1 byte to the Sacara SIL code at a given offset (which start from 0). It pops from the stack:
* the offset of the VM IP
* the byte to write

### NATIVE WRITE
*Mnemonic*: **nwrite**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction allows to write 1 byte to the native memory space. It pops from the stack:
* the native address where to write the value
* the byte to write

### GETIP
*Mnemonic*: **getip**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction push into the stack the current VM IP. This value is the offset of the instruction that follow *getip*.

### JUMP
*Mnemonic*: **jump**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset. It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF LESS
*Mnemonic*: **jumpifl**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF LESS OR EQUAL
*Mnemonic*: **jumpifle**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF GREATER
*Mnemonic*: **jumpifg**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF GREATER OR EQUAL
*Mnemonic*: **jumpifge**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### ALLOCA
*Mnemonic*: **alloca**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction allocates a given number of DOUBLE WORD to be used as storage for local variables. The number accepted can be considered as the number of local variables needed inside the current function. This instruction is mostly used by the assembler. It pops from the stack:
* The number of DOUBLE WORD to allocate for the local variables

### BYTE
*Mnemonic*: **byte <byte value>, ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction but a command for the Assembler that instruct it to emit the specified BYTE in the code. It is also possible to specify more that one value by separating it with a comma (,). This command also allows to specify a string. The string must be enclosed in double quote. An example of usage is the following one:

```
byte "this is a string",0x00
```

### WORD
*Mnemonic*: **word <word value>, ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction, but a command for the Assembler that instruct it to emit the specified WORD in the code. It is also possible to specify more that one value by separating it with a comma (,).

### DOUBLE WORD
*Mnemonic*: **dword <double word value>, ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction, but a command for the Assembler that instruct it to emit the specified DOUBLE WORD in the code. It is also possible to specify more that one value by separating it with a comma (,).

### HALT
*Mnemonic*: **halt.**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This instruction tell the VM to stop the execution.

### CMP
*Mnemonic*: **cmp**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction compares two values from the stack and update the internal flag accordingly. This flag is then used to decide if a jump in necessary or not. It pops from the stack:
* The first value to compare
* The second value to compare

### GETSP
*Mnemonic*: **getsp**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction retrieves the current value of the base of the stack and push it into the stack.

### STACK WRITE
*Mnemonic*: **swrite**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction writes a specific DOUBLE WORD to the given stack offset. It pops from the stack:
* The offset (as an index) of the stack location
* The DOUBLE WORD to write

### STACK READ
*Mnemonic*: **sread**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction reads a DOUBLE WORD from a specified stack offset and push the result back on top of the stack. It pops from the stack:
* The offset (as an index) of the stack location to read from

### SUB
*Mnemonic*: **sub**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction substracts the second value from the first value and push back the result. It pops from the stack:
* The first value
* The second value

### MUL
*Mnemonic*: **sub**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction multiplies the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### DIV
*Mnemonic*: **div**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction divides the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### AND
*Mnemonic*: **and**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *and* of the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### SHIFT RIGHT
*Mnemonic*: **shiftr**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *shift right* on the first argument, for a number of times equals to the second argument and push the result back into the stack. It pops from the stack:
* The value to shift
* The number of times to shift the value

### SHIFT LEFT
*Mnemonic*: **shiftl**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *shift left* on the first argument, for a number of times equals to the second argument and push the result back into the stack. It pops from the stack:
* The value to shift
* The number of times to shift the value

### OR
*Mnemonic*: **or**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *or* between the first argument with the second argument and push the result back into the stack. It pops from the stack:
* The first value
* The second value

### NOT
*Mnemonic*: **not**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction executes a bit *not* operation on the argument and push the result back into the stack. It pops from the stack:
* The value to negate

### XOR
*Mnemonic*: **xor**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *xor* between the first argument and the second argument, and push the result back into the stack. It pops from the stack:
* The first argument
* The second argument

### NOR
*Mnemonic*: **nor**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *nor* between the first argument and the second argument, and push the result back into the stack. It pops from the stack:
* The first argument
* The second argument

### SETIP
*Mnemonic*: **setip**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction modifies the value of the Instruction Pointer with the argument passed. It pops from the stack:
* The new value to assign to the IP

### SETSP
*Mnemonic*: **setsp**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction modifies the value of the Stack Base Pointer with the argument passed. It pops from the stack:
* The new value to assign to the SP base
