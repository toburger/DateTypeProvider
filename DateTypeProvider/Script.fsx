#r @".\bin\Debug\FSharp.DateTypeProvider.dll"

open DateProvider

printfn "%A" <| Date.``2013``.February.``01``.ToDateTime()

printfn "%A" <| Date.``2013``.January.``31``.ToDateTime()

printfn "%A" <| Date.``2013``.``04``.``30``.ToDateTimeOffset()

printfn "%O" <| Date.``2000``.``02``.``29``.ToDateTimeOffset()

//printfn "%A" <| Date.``2002``.``02``.``29``
