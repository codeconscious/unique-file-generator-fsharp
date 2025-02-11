namespace UniqueFileGenerator.Console

open System

module Printing =
    let printColor color msg =
        match color with
        | Some c ->
            Console.ForegroundColor <- c
            printfn $"%s{msg}"
            Console.ResetColor()
        | None -> printfn $"%s{msg}"

module Main =
    open Printing

    [<EntryPoint>]
    let main args =
        let watch = Startwatch.Library.Watch()
        let validatedArgs = ArgValidation.startValidation args

        match validatedArgs with
        | Ok a ->
            StringGenerator.generateMultiple 128 a.FileCount
                |> Array.iter (fun x ->
                    x |> printColor (Some ConsoleColor.Cyan))
            StringGenerator.generateGuids a.FileCount
                |> Array.iter (fun x ->
                    x |> printColor (Some ConsoleColor.Blue))
            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1
