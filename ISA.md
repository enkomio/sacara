# Architecture

_Sacara_ interpreter is implemented by considering a stack based VM, this means that most of the instructions take their arguments from the stack. It also supports local variables that are saved in a different memory area. Both local variables and stack frame are valid only in the current function execution context. Once that you return from a function their values is lost.

Find below a diagram of the _Sacara_ architecture:

<img src="https://raw.githubusercontent.com/enkomio/media/master/sacara/sacara_architecture.png"/>

All values stored inside the stack and local variables are considered as *DOUBLE WORD* in size. However, some instruction (as *read* or *write*) will convert the DOUBLE WORD value to a BYTE value.

The main entry point is a function named **main**.

### Assembling a script
In order to assemble a *Sacara* script you have to use the **SacaraAsm** utility. The utility support various kind of obfuscation options (not all yet implemented), print the help to know more.

The Sacara source code is called **SIL** which stands for **Sacara Intermediate Language**.

You can create **SIL** code also programmatically. For an example of usage take a look at <a href="https://github.com/enkomio/sacara/blob/master/Src/Examples/AssembleManagedInstructions/Program.fs#L16"><i>this example</i></a>. All the supported functions have the same name as the one from the Instruction set below. The only notable differences are: 
* **and** => **_and**
* **or** => **_or**
* **not** => **_not**

# Functions, labels, variables and comments
*Sacara* allows to define functions, to reference labels and to add comments to your code.

### Functions
A function is defined with the **proc** keyword followed by the function name. All functions must end with the **endp** keyword. Find below an example of definition:

```
proc main
    push 0x123
    halt
endp
```

Starting from version **2.3** you can also define the function parameters. The parameter will be considered as local variables that you can reference in tour code. Find below an example:

```
proc main
    push 0x123
    push 0x456
    push 2
    push sum_numbers
    call
    halt
endp

proc sum_numbers(num1, num2)
    push num1
    push num2
    add
    ret
endp
```


### External script
From version **2.3** you can include an external script in your source code by using the **include** keyword following by a string of the file path to include. This is useful to better organize your code. During the program assembling, the _include_ statement will include the content of the specified file. Find below an example:

_File **utility.sacara**_

```
proc sum_numbers(num1, num2)
    push num1
    push num2
    add
    ret
endp
```

_File **main.sacara**_

```
include "utility.sacara"

proc main
    push 0x123
    push 0x456
    push 2
    push sum_numbers
    call
    halt
endp
```

### Label
You can define a **label** inside your code. In *Sacara* each *label* is absolute, this mean that you can reference a label defined in a function from an external function. This is useful to reference data defined in an external function which purpose is to declare all the global variables.

This implies that all label names in your code must be unique. The function name is also considered a label, and you can reference it in the same exact way. Find below an example of label definition and referencing:

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

### Variables
*Sacara VM* supports the definition of variables. It is not necessary to pre-declare them, sacara assembler will scan your code and will allocates the necessary space in the stack. All variables are local. A valid variable name start with a letter and then can contains any numbers of letters, numbers or the _ character. Find below an example of variable usage:

```
proc main
    inc my_variable
    push my_variable
    halt
endp
```

When you want to retrieve the value of a local variable you have to specify it via its numeric index. This value is computed according to the order in which the variable appears in the code, starting from the top of the code and parsing each statement.

Startgin from version **2.3** you can specify the local variable offset. To do so, you must specify the **offset value** followed by the **#** character and the **variable name** as in the following example:

```
proc main
    pop 0#first_variable
    pop 1#second_variable
    push first_variable
    halt
endp
```

In the example above _first_variable_ value will be stored at offset 0. If no offset is specified _Sacara_ will try to find the correct one or create a new one. Be aware that _Sacara_ will reorder your offset, so if you have _0#first_variable_ and _9#second_variable_, the seconda variable will be reorder as _1#second_variable_. This feature is used internally by the _Sacara_ assembler and most of the time can be ignored by the user.

### Comments
You can insert comments in your code to make it more understandable. 

#### Multiple lines comment
*Sacara* support multi lines comment which starts with the string **/\*** and ends with the string **\*/** (this is the same exact pattern used in the C programming language). Find below an example of comment usages:

```
proc main
    push 0x123   /* push second argument */
    push 0x456   /* push first argument */
    add          /* sum */
    halt         /* stop execution */
endp
```

#### Single line comment
Starting from version **2.3** you can also use single line comment by using the character **//**. After that pattern all the remaining text will be considered a comment until the end of the line. Find below an example:

```
proc main
    push 0x123   // push second argument
    push 0x456   // push first argument
    add          // sum
    halt         // stop execution
endp
```

# Instruction set
*Sacara* supports a good amount of instructions. Each instruction can accept 0 or more arguments. Most of the instructions (except for *push* and *pop*) will get the arguments from the stack. When we refer to the first argument, it is the first value that is popped from the stack. The following image show an example of stack layout after pushing the given arguments:

<img src="https://raw.githubusercontent.com/enkomio/media/master/sacara/sacara_stack.png" />

### RET
<hr/>

*Mnemonic*: **ret**

*Popped Arguments*: **0**

*Pushed Arguments*: **at most one**

This instruction returns from a function, if there is a value on top of the stack it is pushed in the caller stack. The function return address is saved on top of the caller stack. When the function returns all the stack and local variables are destroyed.  

### NOP
<hr/>

*Mnemonic*: **nop**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This instruction does nothing, just increase the Instruction Pointer.

### ADD
<hr/>

*Mnemonic*: **add**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction add two values and push back the result. It pops from the stack: 
* first operand
* second operand

### PUSH
<hr/>

*Mnemonic*: **push [imm/label/local variable]**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction accepts a parameter taken from the bytecode and push the value on top of the stack. It can be used with an immutable, with a label or with a local var. Find below an example of usage:

```
proc main
my_label(my_var):
    push my_label         /* push the offset of the label */
    push 0x123            /* push the immediate value 0x123 */
    push my_var           /* push the value stored in my_var */
    push my_func          /* push the offset of the my_func function */
endp

proc my_func
    ret
endp
```

### POP
<hr/>

*Mnemonic*: **pop [local variable]**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction pop from the stack a value and save the result into a local variable.

### CALL
<hr/>

*Mnemonic*: **call**

*Popped Arguments*: **at least 2**

*Pushed Arguments*: **0**

This instruction allows to call a user defined method. It pops from the stack: 
* the name of the method to call
* the number of argument to push in the new stack frame
* the arguments.

### NATIVE CALL
<hr/>

*Mnemonic*: **ncall**

*Popped Arguments*: **at least 2**

*Pushed Arguments*: **1**

This instruction allows to call a *native* method outside of the VM. It pops from the stack:
* the address of the method to call
* the number of argument to push in the stack
* the arguments to the method

When the native method returns the value of the native *EAX* register is pushed on top of the stack. The native function call is agnosting in regarding to the used calling convention.

### READ
<hr/>

*Mnemonic*: **read**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction allows to read data from the Sacara SIL code at a given offset (which start from 0) and push the result into the stack. It pops from the stack:
* the offset of the VM IP
* the type of the data to read, where (1 = byte, 2 = word, 3 = dword)

### NATIVE READ
<hr/>

*Mnemonic*: **nread**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction allows to read data from the native memory space and push the result into the stack. It pops from the stack:
* the native address to read
* the type of the data to read, where (1 = byte, 2 = word, 3 = dword)

### WRITE
<hr/>

*Mnemonic*: **write**

*Popped Arguments*: **3**

*Pushed Arguments*: **0**

This instruction allows to write data to the Sacara SIL code at a given offset (which start from 0). It pops from the stack:
* the offset of the VM IP
* the byte to write
* the type of the data to write, where (1 = byte, 2 = word, 3 = dword)

### NATIVE WRITE
<hr/>

*Mnemonic*: **nwrite**

*Popped Arguments*: **3**

*Pushed Arguments*: **0**

This instruction allows to write data to the native memory space. It pops from the stack:
* the native address where to write the value
* the byte to write
* the type of the data to write, where (1 = byte, 2 = word, 3 = dword)

### GETIP
<hr/>

*Mnemonic*: **getip**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction push into the stack the current VM IP. This value is the offset of the instruction that follow *getip*.

### CMP
<hr/>

*Mnemonic*: **cmp**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction compares two values from the stack and update the internal opaque flags accordingly. The flags are used by a _JXX_ instruction to decide if it must jump or not. It pops from the stack:
* The first value to compare
* The second value to compare

### JUMP
<hr/>

*Mnemonic*: **jump**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset. It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF LESS
<hr/>

*Mnemonic*: **jumpifl**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF LESS OR EQUAL
<hr/>

*Mnemonic*: **jumpifle**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF GREATER
<hr/>

*Mnemonic*: **jumpifg**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### JUMP IF GREATER OR EQUAL
<hr/>

*Mnemonic*: **jumpifge**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction jump to a given offset according to the result of a previous comparison (see *CMP* instruction). It pops from the stack:
* The offset of the VM IP to jump

### ALLOCA
<hr/>

*Mnemonic*: **alloca**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction allocates a given number of DOUBLE WORD to be used as storage for local variables. The number accepted can be considered as the number of local variables needed inside the current function. This instruction is mostly used by the assembler. It pops from the stack:
* The number of DOUBLE WORD to allocate for the local variables

### BYTE
<hr/>

*Mnemonic*: **byte [byte value], ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction but a command for the Assembler that instruct it to emit the specified BYTE in the code. It is also possible to specify more that one value by separating it with a comma (,). This command also allows to specify a string. The string must be enclosed in double quote. An example of usage is the following one:

```
byte "this is a string",0x00
```

### WORD
<hr/>

*Mnemonic*: **word [word value], ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction, but a command for the Assembler that instruct it to emit the specified WORD in the code. It is also possible to specify more that one value by separating it with a comma (,).

### DOUBLE WORD
<hr/>

*Mnemonic*: **dword [double word value], ...**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This is not a real instruction, but a command for the Assembler that instruct it to emit the specified DOUBLE WORD in the code. It is also possible to specify more that one value by separating it with a comma (,).

### HALT
<hr/>

*Mnemonic*: **halt.**

*Popped Arguments*: **0**

*Pushed Arguments*: **0**

This instruction tell the VM to stop the execution.

### GETSP
<hr/>

*Mnemonic*: **getsp**

*Popped Arguments*: **0**

*Pushed Arguments*: **1**

This instruction retrieves the current value of the base of the stack and push it into the stack.

### STACK WRITE
<hr/>

*Mnemonic*: **swrite**

*Popped Arguments*: **2**

*Pushed Arguments*: **0**

This instruction writes a specific DOUBLE WORD to the given stack offset. It pops from the stack:
* The offset (as an index) of the stack location
* The DOUBLE WORD to write

### STACK READ
<hr/>

*Mnemonic*: **sread**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction reads a DOUBLE WORD from a specified stack offset and push the result back on top of the stack. It pops from the stack:
* The offset (as an index) of the stack location to read from

### SUB
<hr/>

*Mnemonic*: **sub**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction substracts the second value from the first value and push back the result. It pops from the stack:
* The first value
* The second value

### MUL
<hr/>

*Mnemonic*: **sub**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction multiplies the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### DIV
<hr/>

*Mnemonic*: **div**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction divides the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### AND
<hr/>

*Mnemonic*: **and**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *and* of the first value with the second value and push back the result. It pops from the stack:
* The first value
* The second value

### SHIFT RIGHT
<hr/>

*Mnemonic*: **shiftr**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *shift right* on the first argument, for a number of times equals to the second argument and push the result back into the stack. It pops from the stack:
* The value to shift
* The number of times to shift the value

### SHIFT LEFT
<hr/>

*Mnemonic*: **shiftl**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *shift left* on the first argument, for a number of times equals to the second argument and push the result back into the stack. It pops from the stack:
* The value to shift
* The number of times to shift the value

### OR
<hr/>

*Mnemonic*: **or**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *or* between the first argument with the second argument and push the result back into the stack. It pops from the stack:
* The first value
* The second value

### NOT
<hr/>

*Mnemonic*: **not**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction executes a bit *not* operation on the argument and push the result back into the stack. It pops from the stack:
* The value to negate

### XOR
<hr/>

*Mnemonic*: **xor**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *xor* between the first argument and the second argument, and push the result back into the stack. It pops from the stack:
* The first argument
* The second argument

### NOR
<hr/>

*Mnemonic*: **nor**

*Popped Arguments*: **2**

*Pushed Arguments*: **1**

This instruction executes a bit *nor* between the first argument and the second argument, and push the result back into the stack. It pops from the stack:
* The first argument
* The second argument

### SETIP
<hr/>

*Mnemonic*: **setip**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction modifies the value of the Instruction Pointer with the argument passed. It pops from the stack:
* The new value to assign to the IP

### SETSP
<hr/>

*Mnemonic*: **setsp**

*Popped Arguments*: **1**

*Pushed Arguments*: **0**

This instruction modifies the value of the Stack Base Pointer with the argument passed. It pops from the stack:
* The new value to assign to the SP base

### INCREMENT
<hr/>

*Mnemonic*: **inc**

*Popped Arguments*: **1**

*Pushed Arguments*: **1**

This instruction pop the value from the stack, increments its value by 1 and push back the result in the stack.
