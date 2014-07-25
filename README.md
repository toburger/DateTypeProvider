DateTypeProvider
================

F# date type provider which provides strong type checking for dates.

Reason for the Type Provider
----------------------------

After watching this video https://vimeo.com/97349221 from Scott Meyers
where he showed an example of a strongly typed datetime class
I thought this is a good fit for an F# Type Provider.

Example
-------
```fsharp
open DateProvider

// dates can be represented with month digits
Date.``2000``.``02``.``29``.ToDateTime()
// or with month names
Date.``2000``.February.``29``.ToDateTime()

// does not compile
Date.``2002``.``13``.``10``.ToDateTime()
Date.``2002``.``02``.``29``.ToDateTime()

// you can also return a DateTimeOffset
Date.``2002``.``13``.``10``.ToDateTimeOffset()

// you can also provide a century parameter
type C21 = Date<century = Century.``21``> // returns years from 2001 to 2100
printfn "%A" <| C21.``2012``.November.``01``.ToDateTime()

type C18 = Date<century = Century.``18``> // returns years from 1701 to 1800
printfn "%A" <| C18.``1789``.July.``14``.ToDateTime()

// you can force a century even if it is not defined in the enum
let [<Literal>] c80: Century = enum 80
type C80 = Date<century = c80>
printfn "%A" <| C80.``7908``.July.``14``.ToDateTime()

// providing a Century value which is not in the valid range (1 to 99) the provider falls back to the current century
let [<Literal>] c100: Century = enum 100
type C100 = Date<century = c100>
printfn "%A" <| C100.``2004``.July.``14``.ToDateTime()
```

Working with NodaTime
---------------------

The Type Provider returns a ```Date``` record with the Fields ```Year```, ```Month``` and ```Day```. You can call the methods ```ToDateTime()``` and ```ToDateTimeOffset(?offset)``` to return a ```DateTime``` or a ```DateTimeOffset```.    
Likewise you can write your own extension method to return a NodaTime ```LocalDate```.


```fsharp
open NodaTime

type FSharp.DateTypeProvider.Date with
    member self.ToLocalDate() =
        LocalDate(self.Year, self.Month, self.Day)

printfn "%A" <| Date.``2013``.February.``01``.ToLocalDate()
```
