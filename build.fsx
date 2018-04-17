#I @"packages/build/FAKE/tools/"
#r @"FakeLib.dll"

open Fake

let project = "FSharp.DateTypeProvider"
let authors = ["Tobias Burger"]
let description = "F# date type provider which provides strong type checking for dates."

let desiredSdkVersion = "2.1.100"
let mutable sdkPath = None
let getSdkPath() = (defaultArg sdkPath "dotnet")

printfn "Desired .NET SDK version = %s" desiredSdkVersion
printfn "DotNetCli.isInstalled() = %b" (DotNetCli.isInstalled())
let useMsBuildToolchain = environVar "USE_MSBUILD" <> null

if DotNetCli.isInstalled() then
    let installedSdkVersion = DotNetCli.getVersion()
    printfn "The installed default .NET SDK version reported by FAKE's 'DotNetCli.getVersion()' is %s" installedSdkVersion
    if installedSdkVersion <> desiredSdkVersion then
        match environVar "CI" with
        | null ->
            if installedSdkVersion > desiredSdkVersion then
                printfn "*** You have .NET SDK version '%s' installed, assuming it is compatible with version '%s'" installedSdkVersion desiredSdkVersion
            else
                printfn "*** You have .NET SDK version '%s' installed, we expect at least version '%s'" installedSdkVersion desiredSdkVersion
        | _ ->
            printfn "*** The .NET SDK version '%s' will be installed (despite the fact that version '%s' is already installed) because we want precisely that version in CI" desiredSdkVersion installedSdkVersion
            sdkPath <- Some (DotNetCli.InstallDotNetSDK desiredSdkVersion)
else
    printfn "*** The .NET SDK version '%s' will be installed (no other version was found by FAKE helpers)" desiredSdkVersion
    sdkPath <- Some (DotNetCli.InstallDotNetSDK desiredSdkVersion)

let bindir = "./bin"

Target "Clean" <| fun _ ->
    seq {
        yield bindir
        yield! !!"**/bin"
        yield! !!"**/obj"
    } |> CleanDirs

Target "Default" DoNothing

let buildProjs =
    [ "DateTypeProvider/FSharp.DateTypeProvider.DesignTime.fsproj"
      "DateTypeProvider/FSharp.DateTypeProvider.fsproj" ]

Target "Build" <| fun _ ->
    buildProjs |> Seq.iter (fun proj ->
        DotNetCli.RunCommand
            (fun p -> { p with ToolPath = getSdkPath () })
            (sprintf "build -c Release \"%s\"" proj))

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

Target "NuGet" <| fun _ ->
    NuGet (fun p ->
        { p with
            Authors = authors
            Project = project
            Description = description
            OutputPath = "bin" })
        "nuget/FSharp.DateTypeProvider.nuspec"

"Clean"
==> "Build"
==> "IntegrationTests"
==> "NuGet"
==> "Default"

RunTargetOrDefault "Default"
