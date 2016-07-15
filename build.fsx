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

Target "IntegrationTest" <| fun _ ->
    Log "Executing: " (!! ("Scripts" @@ "Test.fsx"))
    let success, msgs = executeFSI "Scripts" "Test.fsx" []
    traceHeader "Output: "
    for msg in msgs do
        trace msg.Message
    traceLine ()
    if not success then
        failwith "Error while executing integration test script"

"Clean"
==> "Build"
==> "IntegrationTest"
==> "Default"

RunTargetOrDefault "Default"
