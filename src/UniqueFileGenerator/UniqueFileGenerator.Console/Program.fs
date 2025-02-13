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
    open ArgValidation
    open Printing

    // TODO: Move elsewhere.
    let private prepend args =
        match args.Flags |> Map.tryFindKey (fun x _ -> x = "-p") with
        | Some key ->
            let text = args.Flags |> Map.find key
            fun fileName -> $"%s{text}%s{fileName}"
        | None -> id

    let private appendExt args =
        match args.Flags |> Map.tryFindKey (fun x _ -> x = "-e") with
        | Some key ->
            let ext = args.Flags |> Map.find key
            fun fileName -> $"%s{fileName}.%s{ext}"
        | None -> id

    [<EntryPoint>]
    let main args =
        let watch = Startwatch.Library.Watch()
        let validatedArgs = ArgValidation.startValidation args

        match validatedArgs with
        | Ok a ->
            let ext = appendExt a
            let pre = prepend a
            let combined = ext >> pre

            StringGenerator.generateMultiple 128 a.FileCount
                |> Array.iter (fun x ->
                    x
                    |> combined
                    |> printColor (Some ConsoleColor.Cyan))
            StringGenerator.generateGuids a.FileCount
                |> Array.iter (fun x ->
                    x
                    |> combined
                    |> printColor (Some ConsoleColor.Blue))
            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1

