namespace FSharp.DateTypeProvider

open System
open System.Globalization
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open ProviderImplementation.ProvidedTypes
open DateProvider

type Time =
    { Hour : int
      Minute : int
      Second : int }
    override self.ToString() =
        sprintf "%02d:%02d:%02d"
                self.Hour self.Minute self.Second
    member self.ToTimeSpan() =
        TimeSpan(self.Hour, self.Minute, self.Second)

[<TypeProvider>]
type TimeTypeProvider() as self =
    inherit TypeProviderForNamespaces()

    let thisAssembly = Assembly.GetExecutingAssembly()
    let rootNamespace = "DateProvider"
    
    let secondProp (hour, minute, second) =
        let getter _ = <@@ { Hour = hour; Minute = minute; Second = second } @@>
        let prop = ProvidedProperty(propertyName = second.ToString("d2"),
                                    propertyType = typeof<Time>,
                                    IsStatic = true,
                                    GetterCode = getter)
        prop.AddXmlDocDelayed(fun () -> (DateTime(1, 1, 1, hour, minute, second)).ToLongTimeString())
        prop

    let minuteType (hour, (minute: int)) =
        let t = ProvidedTypeDefinition(minute.ToString("d2"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for second in 0..59 do
                yield secondProp (hour, minute, second) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(1, 1, 1, hour, minute, 0)).ToLongTimeString())
        t

    let hourType (hour: int) =
        let t = ProvidedTypeDefinition(hour.ToString("d2"), Some typeof<obj>)
        t.AddMembersDelayed(fun () ->
            [ for minute in 0..59 do
                yield minuteType (hour, minute) ])
        t.AddXmlDocDelayed(fun () -> (DateTime(1, 1, 1, hour, 0, 0)).ToLongTimeString())
        t

    let containerType =
        let t = ProvidedTypeDefinition(thisAssembly, rootNamespace, "Time", Some typeof<obj>)
        t.AddMembersDelayed(fun () -> 
            [ for hour in 0..23 do
                yield hourType hour ])
        t

    do self.AddNamespace(rootNamespace, [ containerType ])

[<TypeProviderAssembly>]
do ()
