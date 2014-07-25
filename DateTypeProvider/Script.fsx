#r @".\bin\Debug\FSharp.DateTypeProvider.dll"

open DateProvider

printfn "%A" <| Date.``2013``.February.``01``.ToDateTime()

printfn "%A" <| Date.``2013``.January.``31``.ToDateTime()

printfn "%A" <| Date.``2013``.``04``.``30``.ToDateTimeOffset()

printfn "%O" <| Date.``2004``.``02``.``29``.ToDateTimeOffset()

//printfn "%A" <| Date.``2002``.``02``.``29``

type C21 = Date<century = Century.``21``>
printfn "%A" <| C21.``2012``.November.``01``.ToDateTime()

type C18 = Date<century = Century.``18``>
printfn "%A" <| C18.``1789``.July.``14``.ToDateTime()
