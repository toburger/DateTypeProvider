#r @"..\bin\lib\net45\FSharp.DateTypeProvider.dll"

open DateProvider

printfn "%A" <| DateTime.``2013``.February.``01``.``23``.``30``.``15``.ToDateTime()

printfn "%A" <| DateTime.``2013``.January.``31``.``23``.``30``.``15``.ToDateTime()

printfn "%A" <| DateTime.``2013``.``04``.``30``.``23``.``30``.``15``.ToDateTimeOffset()

printfn "%O" <| DateTime.``2004``.``02``.``29``.``23``.``30``.``15``.ToDateTimeOffset()

//printfn "%A" <| DateTime.``2002``.``02``.``29``.``00``.``00``.``00``

type C21 = DateTime<century = Century.``21``>
printfn "%A" <| C21.``2012``.November.``01``.``23``.``30``.``15``.ToDateTime()

type C18 = DateTime<century = Century.``18``>
printfn "%A" <| C18.``1789``.July.``14``.``23``.``30``.``15``.ToDateTime()

// you can force a century even if it is not defined in the enum
let [<Literal>] c80: Century = enum 80
type C80 = DateTime<century = c80>
printfn "%A" <| C80.``7908``.July.``14``.``23``.``30``.``15``.ToDateTime()

// providing an invalid Century falls back to the current century
let [<Literal>] c100: Century = enum 100
type C100 = DateTime<century = c100>
printfn "%A" <| C100.``2004``.July.``14``.``23``.``30``.``15``.ToDateTime()