#I @"packages/build/FAKE/tools/"
#r @"FakeLib.dll"

open Fake

let buildDir = "./build"

Target "Clean" <| fun _ ->
    CleanDir buildDir

Target "Default" DoNothing

Target "Build" <| fun _ ->
    !! "**/*.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "

"Clean"
==> "Build"
==> "Default"

RunTargetOrDefault "Default"
