namespace UniqueFileGenerator.Console

open System
open System.Globalization

module Utilities =
    let formatInt (i: int) : string =
        i.ToString("#,##0", CultureInfo.InvariantCulture)

    let formatInt64 (i: int64) : string =
        i.ToString("#,##0", CultureInfo.InvariantCulture)

    let formatFloat (f: float) : string =
        f.ToString("#,##0.00", CultureInfo.InvariantCulture)

    let inline (>=<) x (floor, ceiling) = x >= floor && x <= ceiling

    let tryParseInt (input: string) : int option =
        match Int32.TryParse input with
        | true, i -> Some i
        | false, _ -> None

    let parseInRange (floor, ceiling) (x: string) : Result<int, unit> =
        match tryParseInt x with
        | Some i when (>=<) i (floor, ceiling) ->
            Ok i
        | _ ->
            Error ()


