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

let integrationTest scriptFile =
    Log "Executing: " (!! ("Scripts" @@ scriptFile))
    let success, msgs = executeFSI "Scripts" scriptFile []
    traceHeader "Output: "
    for msg in msgs do
        trace msg.Message
    traceLine ()
    if not success then
        failwith "Error while executing integration test script"

Target "IntegrationTests" <| fun _ ->
    integrationTest "DateTest.fsx"
    integrationTest "TimeTest.fsx"
    integrationTest "DateTimeTest.fsx"

"Clean"
==> "Build"
==> "IntegrationTests"
==> "Default"

RunTargetOrDefault "Default"
