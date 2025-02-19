namespace UniqueFileGenerator.Console

open System

module Printing =
    // Be careful with color because we don't know users' terminal color schemes.
    let private printLineColor color msg =
        match color with
        | Some c ->
            Console.ForegroundColor <- c
            printfn $"%s{msg}"
            Console.ResetColor()
        | None -> printfn $"%s{msg}"

    let printLine msg =
        printLineColor None msg

    let printError msg =
        printLineColor (Some ConsoleColor.Red) msg

    let printWarning msg =
        printLineColor (Some ConsoleColor.Yellow) msg

    let printEmptyLine () =
        printLineColor None String.Empty
