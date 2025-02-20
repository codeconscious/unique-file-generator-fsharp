namespace UniqueFileGenerator.Console

open System

module Printing =
    // Be careful with color because we don't know the user's terminal color scheme,
    // so it's easy to unintentionally output invisible or hard-to-read text.
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

    let printEmptyLine () =
        printLineColor None String.Empty
