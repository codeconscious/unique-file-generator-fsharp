namespace UniqueFileGenerator.Console

open System

module Printing =
    let printLineColor color msg =
        match color with
        | Some c ->
            Console.ForegroundColor <- c
            printfn $"%s{msg}"
            Console.ResetColor()
        | None -> printfn $"%s{msg}"

    let printLine msg =
        printLineColor None msg

    let printEmptyLine () =
        printLineColor None String.Empty
