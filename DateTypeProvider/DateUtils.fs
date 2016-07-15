module internal DateUtils

open System
open System.Globalization

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
