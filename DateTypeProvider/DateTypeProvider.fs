namespace FSharp.DateTypeProvider

open System
open System.Globalization
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open ProviderImplementation.ProvidedTypes
open DateProvider

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

[<TypeProvider>]
type DateTypeProvider() as self =
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "DateProvider"
    
    let daysProp (year, month, day) =
        let getter _ = <@@ { Year = year; Month = month; Day = day } @@>
        let prop = ProvidedProperty(propertyName = day.ToString("d2"),
                                    propertyType = typeof<Date>,
                                    IsStatic = true,
                                    GetterCode = getter)
        prop.AddXmlDocDelayed(fun () -> (DateTime(year, month, day)).ToLongDateString())
        prop

    let monthType className (year, month) =
        let t = ProvidedTypeDefinition(className month, Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            let days = DateUtils.getDaysInMonth (year, month)
            [ for day in 1..days -> daysProp (year, month, day) ])
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
        let t = ProvidedTypeDefinition(thisAssembly, rootNamespace, "Date", Some typeof<obj>)
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
