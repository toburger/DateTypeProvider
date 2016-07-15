namespace DateProvider

open System

type Century =
    | ``01`` =  1 | ``02`` =  2 | ``03`` =  3 | ``04`` =  4 | ``05`` =  5
    | ``06`` =  6 | ``07`` =  7 | ``08`` =  8 | ``09`` =  9 | ``10`` = 10
    | ``11`` = 11 | ``12`` = 12 | ``13`` = 13 | ``14`` = 14 | ``15`` = 15
    | ``16`` = 16 | ``17`` = 17 | ``18`` = 18 | ``19`` = 19 | ``20`` = 20
    | ``21`` = 21 | ``22`` = 22 | ``23`` = 23 | ``24`` = 24 | ``25`` = 25
    | ``26`` = 26 | ``27`` = 27 | ``28`` = 28 | ``29`` = 29 | ``30`` = 30
    | ``31`` = 31 | ``32`` = 32 | ``33`` = 33 | ``34`` = 34 | ``35`` = 35
    | ``36`` = 36 | ``37`` = 37 | ``38`` = 38 | ``39`` = 39 | ``40`` = 40

namespace FSharp.DateTypeProvider

open System

type Date =
    { Year : int
      Month : int
      Day : int }
    override self.ToString() =
        sprintf "%04d-%02d-%02d"
                self.Year self.Month self.Day
    member self.ToDateTime() =
        DateTime(self.Year, self.Month, self.Day)
    member self.ToDateTimeOffset(?offset) =
        let offset = offset |> defaultArg <| TimeSpan.Zero
        DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, offset)

type Time =
    { Hour : int
      Minute : int
      Second : int }
    override self.ToString() =
        sprintf "%02d:%02d:%02d"
                self.Hour self.Minute self.Second
    member self.ToTimeSpan() =
        TimeSpan(self.Hour, self.Minute, self.Second)

type DateTime =
    { Year : int
      Month : int
      Day : int
      Hour : int
      Minute : int
      Second : int }
    override self.ToString() =
        sprintf "%04d-%02d-%02dT%02d:%02d:%02d"
                self.Year self.Month self.Day
                self.Hour self.Minute self.Second
    member self.ToDateTime() =
        DateTime(self.Year, self.Month, self.Day, self.Hour, self.Minute, self.Second)
    member self.ToDateTimeOffset(?offset) =
        let offset = offset |> defaultArg <| TimeSpan.Zero
        DateTimeOffset(self.Year, self.Month, self.Day, self.Hour, self.Minute, self.Second, offset)
