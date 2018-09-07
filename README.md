# Sacara - An IR stack based language aimed at software protection by using a VM software concept
 
_Sacara_ is a programming language very similar to the most common intermediate representation language, like MSIL or the Java bytecode. 
It is execute inside a VM and its main purpose is to make difficult to understand the original purpose of the program.
This make the project well suited for protecting the code from being reverse enginnering. 

Of course nothing will stop an highly skilled reverse engineer :)

## Release Download
 - [Source code][1]
 - [Download binary][2]
 
## Using Sacara

In order to compile a script implemented in the Sacara Intermediate Language (SIL), you have to use the Sacara assembler. 

To run a Sacara compiled script you can use the SacaraRun utility, or embedd the code inside your source code and using the exported APIs to run the SIL in a more controlled environment.

Take a look at the Examples folder for examples of usage.

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
