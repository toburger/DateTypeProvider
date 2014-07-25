namespace FSharp.DateTypeProvider

open System
open System.Globalization
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open ProviderImplementation.ProvidedTypes

module DateUtils =
    let getMonthName =
        let dtfi = DateTimeFormatInfo()
        fun month -> dtfi.GetMonthName(month)

    let getDaysInMonth =
        let calendar = CultureInfo.InvariantCulture.Calendar
        fun (year, month) -> calendar.GetDaysInMonth(year, month)

    let getCentury year =
        int (ceil(float year / 100.))

    let getCurrentCentury () =
        getCentury DateTime.UtcNow.Year

    let getYearsInCentury century =
        let century' = century * 100
        let begin', end' = century' - 99, century'
        [ begin'..end' ]

    let isValidCentury century =
        if   century > 99 then false
        elif century < 1 then false
        else true

type Date =
    { Year : int
      Month : int
      Day : int }
    override self.ToString() = sprintf "%04d.%02d.%02d" self.Year self.Month self.Day

type Century =
    | ``01`` =  1 | ``02`` =  2 | ``03`` =  3 | ``04`` =  4 | ``05`` =  5
    | ``06`` =  6 | ``07`` =  7 | ``08`` =  8 | ``09`` =  9 | ``10`` = 10
    | ``11`` = 11 | ``12`` = 12 | ``13`` = 13 | ``14`` = 14 | ``15`` = 15
    | ``16`` = 16 | ``17`` = 17 | ``18`` = 18 | ``19`` = 19 | ``20`` = 20
    | ``21`` = 21 | ``22`` = 22 | ``23`` = 23 | ``24`` = 24 | ``25`` = 25

[<TypeProvider>]
type DateTypeProvider() as self =
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "DateProvider"

    let containerType = ProvidedTypeDefinition(thisAssembly,
                                               rootNamespace,
                                               "Date",
                                               None,
                                               IsErased = true)
    
    let daysProp (year, month, day) =
        let getter _ = <@@ { Year = year; Month = month; Day = day } @@>
        let prop = ProvidedProperty(propertyName = day.ToString("d2"),
                                    propertyType = typeof<Date>,
                                    IsStatic = true,
                                    GetterCode = getter)
        prop.AddXmlDocDelayed(fun () -> (DateTime(year, month, day)).ToLongDateString())
        prop

    let monthType className (year, month) =
        let t = ProvidedTypeDefinition(className month, None, IsErased = true)
        t.AddMembersDelayed(fun () ->
            let days = DateUtils.getDaysInMonth (year, month)
            [ for day in 1..days -> daysProp (year, month, day) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(year, month, 1)).ToString("MMMM yyyy"))
        t

    let yearType (year: int) =
        let t = ProvidedTypeDefinition(year.ToString("d4"), None, IsErased = true)
        t.AddMembersDelayed(fun () ->
            [ for month in 1..12 do
                yield monthType (fun m -> m.ToString("d2")) (year, month)
                yield monthType (DateUtils.getMonthName) (year, month) ])
        t.AddXmlDocDelayed(fun () -> year.ToString("d4"))
        t
        
    do containerType.AddMembersDelayed(fun () ->
        DateUtils.getCurrentCentury()
        |> DateUtils.getYearsInCentury
        |> List.map yearType)

    let createDateProvider typeName century =
        let t = ProvidedTypeDefinition(thisAssembly, rootNamespace, typeName, baseType = Some typeof<obj>, HideObjectMethods = true)
        t.AddMembersDelayed(fun () ->
            DateUtils.getYearsInCentury century
            |> List.map yearType)
        t

    do containerType.DefineStaticParameters(
        staticParameters = [ProvidedStaticParameter("century", typeof<Century>)],
        apply = (fun typeName parameterValues ->
            match parameterValues with
            | [| :? int as century |] when DateUtils.isValidCentury century -> century
            | _ -> DateUtils.getCurrentCentury()
            |> createDateProvider typeName))

    do self.AddNamespace(rootNamespace, [ containerType ])

[<TypeProviderAssembly>]
do ()

type Date with
    member self.ToDateTime() =
        DateTime(self.Year, self.Month, self.Day)
    member self.ToDateTimeOffset(?offset) =
        let offset = offset |> defaultArg <| TimeSpan.Zero
        DateTimeOffset(self.Year, self.Month, self.Day, 0, 0, 0, offset)

namespace DateProvider

type Century = FSharp.DateTypeProvider.Century
