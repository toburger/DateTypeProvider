namespace FSharp.DateTypeProvider

open System
open System.Globalization
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open ProviderImplementation.ProvidedTypes
open DateProvider

[<TypeProvider>]
type DateTimeTypeProvider() as self =
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "DateProvider"

    let secondProp (year, month, day, hour, minute, second) =
        let getter _ = <@@ { Year = year; Month = month; Day = day; Hour = hour; Minute = minute; Second = second } @@>
        let prop = ProvidedProperty(propertyName = second.ToString("d2"),
                                    propertyType = typeof<FSharp.DateTypeProvider.DateTime>,
                                    IsStatic = true,
                                    GetterCode = getter)
        prop.AddXmlDocDelayed(fun () -> (DateTime(year, month, day, hour, minute, second)).ToString())
        prop

    let minuteType (year, month, day, hour, (minute: int)) =
        let t = ProvidedTypeDefinition(minute.ToString("d2"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for second in 0..59 do
                yield secondProp (year, month, day, hour, minute, second) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(year, month, day, hour, minute, 0)).ToString())
        t

    let hourType (year, month, day, (hour: int)) =
        let t = ProvidedTypeDefinition(hour.ToString("d2"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for minute in 0..59 do
                yield minuteType (year, month, day, hour, minute) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(year, month, day, hour, 0, 0)).ToString())
        t

    let dayType (year, month, (day: int)) =
        let t = ProvidedTypeDefinition(day.ToString("d2"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for hour in 0..23 do
                yield hourType (year, month, day, hour) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(year, month, day)).ToLongDateString())
        t

    let monthType className (year, month) =
        let t = ProvidedTypeDefinition(className month, Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            let days = DateUtils.getDaysInMonth (year, month)
            [ for day in 1..days -> dayType (year, month, day) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(year, month, 1)).ToString("MMMM yyyy"))
        t

    let yearType (year: int) =
        let t = ProvidedTypeDefinition(year.ToString("d4"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for month in 1..12 do
                yield monthType (fun m -> m.ToString("d2")) (year, month)
                yield monthType (DateUtils.getMonthName) (year, month) ])
        t.AddXmlDocDelayed(fun () -> year.ToString("d4"))
        t

    let createDateProvider typeName century =
        let t = ProvidedTypeDefinition(thisAssembly, rootNamespace, typeName, baseType = Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            DateUtils.getYearsInCentury century
            |> List.map yearType)
        t

    let containerType =
        let t = ProvidedTypeDefinition(thisAssembly, rootNamespace, "DateTime", Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            DateUtils.getCurrentCentury()
            |> DateUtils.getYearsInCentury
            |> List.map yearType)
        t.DefineStaticParameters(
            parameters = [ProvidedStaticParameter("century", typeof<Century>)],
            instantiationFunction = (fun typeName parameterValues ->
                match parameterValues with
                | [| :? int as century |] when DateUtils.isValidCentury century -> century
                | _ -> DateUtils.getCurrentCentury()
                |> createDateProvider typeName))
        t

    do self.AddNamespace(rootNamespace, [ containerType ])

[<TypeProviderAssembly>]
do ()
