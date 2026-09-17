namespace UniqueFileGenerator.Console

open System
open System.Globalization

module Utilities =

    /// Checks if a value falls within an inclusive range.
    let inline (>=<) x (floor, ceiling) = x >= floor && x <= ceiling

    let tryParseInt (input: string) : int option =
        match Int32.TryParse input with
        | true,  i -> Some i
        | false, _ -> None

    let tryParseInRange (floor, ceiling) (text: string) : Result<int, unit> =
        match tryParseInt text with
        | Some i when (>=<) i (floor, ceiling) -> Ok i
        | _ -> Error ()

    // Numeric operations.
    type Num =

        static member Format(i: int) : string =
            i.ToString("#,##0", CultureInfo.InvariantCulture)

        static member Format(i: int64) : string =
            i.ToString("#,##0", CultureInfo.InvariantCulture)

        static member Format(f: float) : string =
            f.ToString("#,##0.00", CultureInfo.InvariantCulture)
