# Sacara - A stack based intermediate language aimed at software protection by running in a software VM

 <p align="center">
    <a href="https://github.com/enkomio/sacara/releases/latest"><img alt="Release" src="https://img.shields.io/github/release/enkomio/sacara.svg?svg=true"></a>   
    <a href="https://github.com/enkomio/sacara/blob/master/LICENSE.md"><img alt="Software License" src="https://img.shields.io/badge/License-CC%20BY%204.0-brightgreen.svg"></a>
  </p>
 
_Sacara_ is a programming language very similar to the most common intermediate representation language, like MSIL or the Java bytecode. 
It is executed inside a VM and its main purpose is to make difficult to understand the original purpose of the program.
This make the project well suited for protecting the code from being reverse enginnering. 

Of course nothing will stop an highly skilled reverse engineer :)

## Release Download
 - [Source code][1]
 - [Download binary][2]
 
## Using Sacara

In order to compile a script implemented in the Sacara Intermediate Language (SIL), you have to use the Sacara assembler <a href="https://github.com/enkomio/sacara/tree/master/Src/SacaraAsm">**SacaraAsm**</a>. 

To run a Sacara compiled script you can use the <a href="https://github.com/enkomio/sacara/tree/master/Src/SacaraRun">**SacaraRun**</a> utility, or embedd the code inside your source code and using the exported APIs to run the SIL in a more controlled environment.

The Sacara VM is implemented in the <a href="https://github.com/enkomio/sacara/tree/master/Src/SacaraVm">**SacaraVM**</a> dll.

Find below an example of execution:

<img src="https://raw.githubusercontent.com/enkomio/media/master/sacara/sacara_run.gif" />

For more examples take a look at the <a href="https://github.com/enkomio/sacara/tree/master/Src/Examples">Examples folder</a>.

## Documentation

For documentation related to how the SacaraVM can be used, please read the [ISA][3] page. 

If you are interested in using _Sacara_ in .NET take a look at <a href='https://github.com/enkomio/sacara/blob/master/Src/Examples/DotNetBinding/Program.fs'>this example</a>, which use the <a href='https://github.com/enkomio/sacara/tree/master/Src/ES.SacaraVm'>.NET Sacara Binding (ES.SacaraVm)</a>. In order to use the .NET binding the unmanaged _SacaraVm.dll_ file must be in the same directory as the _ES.SacaraVm.dll_ Assembly file.


I have also published a blog post about using Sacara to hide code intent. You can read it <a href="http://antonioparata.blogspot.com/2018/11/sacara-vm-vs-antivirus-industry.html">here</a>.

## Build Sacara
_Sacara_ is currently developed by using VisualStudio 2017 Community Edition. To build the source code you have to:
* clone the repository
* run ``build.bat``

## Versioning

I used [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/enkomio/sacara/tags). 

## Authors

* **Antonio Parata** - *Core Developer* - [s4tan](https://twitter.com/s4tan)

## License

Sacara is licensed under the [MIT license](LICENSE.TXT).

  [1]: https://github.com/enkomio/sacara/tree/master/Src
  [2]: https://github.com/enkomio/sacara/releases/latest
  [3]: https://github.com/enkomio/sacara/blob/master/ISA.md
