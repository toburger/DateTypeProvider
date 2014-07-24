#r @".\bin\Debug\FSharp.DateTypeProvider.dll"

open DateProvider

printfn "%A" (Date.``2013``.February.``01``)

printfn "%A" (Date.``2013``.January.``31``)

printfn "%A" (Date.``2013``.``04``.``30``)

printfn "%A" (Date.``2000``.``02``.``29``)

//printfn "%A" (Date.``2002``.``02``.``29``)
