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

// you can force a century even if it is not defined in the enum
let [<Literal>] c80: Century = enum 80
type C80 = Date<century = c80>
printfn "%A" <| C80.``7908``.July.``14``.ToDateTime()

// providing an invalid Century falls back to the current century
let [<Literal>] c100: Century = enum 100
type C100 = Date<century = c100>
printfn "%A" <| C100.``2004``.July.``14``.ToDateTime()