DateTypeProvider
================

F# date type provider which provides strong type checking for dates.

Reason for the Type Provider
----------------------------

After watching this video https://vimeo.com/97349221 from Scott Meyers
where he showed an example of a strongly typed datetime class
I thought this is a good fit for an F# Type Provider.


_I had to limit the years because intellisense stopped working when providing too many years. Maybe this can be fixed, but I don't know how._

Example
-------
```fsharp
// dates can be represented with month digits
DateProvider.Date.``2000``.``02``.``29``.ToDateTime()
// or with month names
DateProvider.Date.``2000``.February.``29``.ToDateTime()

// does not compile
DateProvider.Date.``2002``.``13``.``10``.ToDateTime()
DateProvider.Date.``2002``.``02``.``29``.ToDateTime()

// you can also return a DateTimeOffset
DateProvider.Date.``2002``.``13``.``10``.ToDateTimeOffset()

// you can also provide a century parameter
type C21 = Date<century = Century.``21``> // returns years from 2001 to 2100
printfn "%A" <| C21.``2012``.November.``01``.ToDateTime()

type C18 = Date<century = Century.``18``> // returns years from 1701 to 1800
printfn "%A" <| C18.``1789``.July.``14``.ToDateTime()
```

Footnotes
---------

~~I'm not sure if a DateTimeOffset is a good fit for a date with no time information.
It could be better represented by NodaTime's LocalDate class (http://nodatime.org/1.3.x/userguide/localdate-patterns.html).    
I sticked with the DateTimeOffset (for now) because I didn't wanted to add the dependency of NodaTime to the project.~~

It returns a ```Date``` record with the Fields ```Year```, ```Month``` and ```Day```. You can call the methods ```ToDateTime()``` and ```ToDateTimeOffset(?offset)``` to return a ```DateTime``` or a ```DateTimeOffset```.    
Likewise you can write your own extension method to return a NodaTime ```LocalDate```.
