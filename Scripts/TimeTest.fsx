#r @"..\build\FSharp.DateTypeProvider.dll"

open DateProvider

printfn "%A" <| Time.``23``.``30``.``15``.ToTimeSpan()

printfn "%A" <| Time.``00``.``00``.``00``.ToTimeSpan()

//printfn "%A" <| Time.``99``.``02``.``29``
