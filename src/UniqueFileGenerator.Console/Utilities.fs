namespace UniqueFileGenerator.Console

open System.Globalization

module Utilities =
    let formatInt (i: int) : string =
        i.ToString("#,##0", CultureInfo.InvariantCulture)

    let formatInt64 (i: int64) : string =
        i.ToString("#,##0", CultureInfo.InvariantCulture)

    let formatFloat (f: float) : string =
        f.ToString("#,##0.##", CultureInfo.InvariantCulture)


