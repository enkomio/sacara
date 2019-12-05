# Sacara - A stack based intermediate language aimed at software protection by running in a software VM

 <p align="center">
    <a href="https://github.com/enkomio/sacara/releases/latest"><img alt="Release" src="https://img.shields.io/github/release/enkomio/sacara.svg?svg=true"></a>   
    <a href="https://github.com/enkomio/sacara/blob/master/LICENSE.md"><img alt="Software License" src="https://img.shields.io/badge/License-CC%20BY%204.0-brightgreen.svg"></a>
  </p>

Under the _Sacara_ name belongs various projects:
* A programming language very similar to the most common intermediate representation language, like MSIL or the Java bytecode
* An assembler to transalate your _Sacara_ program in a binary format
* An interpreter based on a Virtual Machine stack based
* A .NET binding to use the unmanaged _Sacara_ DLL

_Sacara_ was created  to learn how to create a project suited for protecting the code from being reverse enginnering. The Virtual Machine is implemented in Assembly x86 and contains some anti-analysis features. 

## Documentation
To know how to program in SacaraVM you can have a look at the [ISA][3] page, see the [Examples][4] in the source folder or read the programs ([this][5] and [this][6]) used for testing. 

I have also published some blog posts about how to use _Sacara_ for some basic tasks.

* <a href="http://antonioparata.blogspot.com/2018/11/sacara-vm-vs-antivirus-industry.html">Sacara VM Vs Antivirus Industry</a>.

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

## .NET Binding

If you are interested in using _Sacara_ in .NET take a look at <a href='https://github.com/enkomio/sacara/blob/master/Src/Examples/DotNetBinding/Program.fs'>this example</a>, which use the <a href='https://github.com/enkomio/sacara/tree/master/Src/ES.SacaraVm'>.NET Sacara Binding (ES.SacaraVm)</a>. In order to use the .NET binding the unmanaged _SacaraVm.dll_ file must be in the same directory as the _ES.SacaraVm.dll_ Assembly file.

## Build Sacara
_Sacara_ is currently developed by using VisualStudio 2017 Community Edition (be sure to have the latest version installed). To build the source code you have to:
* have installed <a href="https://www.microsoft.com/net/download">.NET Core SDK</a>
* have installed the <a href="https://blogs.msdn.microsoft.com/vcblog/2017/04/17/windows-desktop-development-with-c-in-visual-studio/">Windows desktop development with c++</a>. If you have installed Visual Studio 2017, by opening the solution (SacaraSln.sln) it should ask automatically if you want to install the missing component
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
  [4]: https://github.com/enkomio/sacara/tree/master/Src/Examples
  [5]: https://github.com/enkomio/sacara/tree/master/Src/EndToEndTests/TestSources/SelfContained
  [6]: https://github.com/enkomio/sacara/tree/master/Src/EndToEndTests/TestSources/Custom
