// --------------------------------------------------------------------------------------
// Sacara FAKE build script
// --------------------------------------------------------------------------------------
#r "paket: groupref FakeBuild //"
#load ".fake/build.fsx/intellisense.fsx"

open System
open System.Reflection
open System.IO
open Fake.DotNet
open Fake.Core
open Fake.IO
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators
 
// The name of the project
let project = "Sacara"

// Short summary of the project
let description = "A VM stack based IR language well suite for code protection."

// List of author names (for NuGet package)
let authors = [ "Enkomio" ]

// Build dir
let buildDir = "./build"

// Package dir
let deployDir = "./deploy"

// Read additional information from the release notes document
let releaseNotesData = 
    let changelogFile = Path.Combine("..", "RELEASE_NOTES.md")
    File.ReadAllLines(changelogFile)
    |> ReleaseNotes.parseAll
    
let releaseVersion = (List.head releaseNotesData)
Trace.log("Build release: " + releaseVersion.AssemblyVersion)

let genFSAssemblyInfo (projectPath) =
    let projectName = Path.GetFileNameWithoutExtension(projectPath)
    let folderName = Path.GetFileName(System.IO.Path.GetDirectoryName(projectPath))
    let fileName = Path.Combine(folderName, "AssemblyInfo.fs")

    AssemblyInfoFile.createFSharp fileName
        [ 
            AssemblyInfo.Title projectName        
            AssemblyInfo.Product project
            AssemblyInfo.Company (authors |> String.concat ", ")
            AssemblyInfo.Description description
            AssemblyInfo.Version (releaseVersion.AssemblyVersion + ".*")
            AssemblyInfo.FileVersion (releaseVersion.AssemblyVersion + ".*")
            AssemblyInfo.InformationalVersion (releaseVersion.NugetVersion + ".*") 
        ]
        
(*
    FAKE targets
*)
Target.create "Clean" (fun _ ->
    Shell.cleanDir buildDir
    Directory.ensure buildDir

    Shell.cleanDir deployDir
    Directory.ensure deployDir
)

Target.create "SetAssemblyInfo" (fun _ ->
    !! "*/**/*.fsproj"
    |> Seq.iter genFSAssemblyInfo    
)

Target.create "Compile" (fun _ ->
    let build(project: String, buildDir: String) =
        Trace.log("Compile: " + project)
        let fileName = Path.GetFileNameWithoutExtension(project)
        let buildAppDir = Path.Combine(buildDir, fileName)
        Directory.ensure buildAppDir

        // build the project
        [project]
        |> MSBuild.runRelease id buildAppDir "Build"
        |> Trace.logItems "AppBuild-Output: "

    // build all projects
    [
        "Sacara"
    ]
    |> List.map(fun projName ->
        let projFile = Path.Combine(projName, projName + ".fsproj")
        Trace.log("Build project: " + projFile)
        projFile
    )
    |> List.iter(fun projectFile -> build(projectFile, buildDir))
)

Target.create "Release" (fun _ ->
    let forbidden = [".pdb"]    
    !! (buildDir + "/**/*.*")         
    |> Seq.filter(fun f -> 
        forbidden 
        |> List.contains (Path.GetExtension(f).ToLowerInvariant())
        |> not
    )
    |> Zip.zip buildDir (Path.Combine(deployDir, "Sacara." + releaseVersion.AssemblyVersion + ".zip"))
)

Target.description "Default Build all artifacts"
Target.create "Default" ignore

(*
    Run task
*)

"Clean"
  ==> "SetAssemblyInfo"
  ==> "Compile"
  ==> "Release"
  ==> "Default"

// start build
Target.runOrDefault "Default"