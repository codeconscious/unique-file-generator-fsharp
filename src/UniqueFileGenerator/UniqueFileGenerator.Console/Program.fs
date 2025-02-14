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

// module IO =


module Main =
    open ArgValidation
    open Printing

    // TODO: Move elsewhere.
    let private prepend args =
        match args.Options.Prefix with
        | Some p -> fun fileName -> $"%s{p}%s{fileName}"
        | None -> id

    let private appendExt args =
        match args.Options.Extension with
        | Some e -> fun fileName -> $"%s{fileName}.%s{e}"
        | None -> id

    [<EntryPoint>]
    let main args =
        let watch = Startwatch.Library.Watch()
        let validatedArgs = ArgValidation.validate args

        match validatedArgs with
        | Ok a ->
            let ext = appendExt a
            let pre = prepend a
            let textProcessing = ext >> pre

            StringGenerator.generateMultiple 128 a.FileCount
                |> Array.iter (fun x ->
                    x
                    |> textProcessing
                    |> printColor (Some ConsoleColor.Cyan))
            StringGenerator.generateGuids a.FileCount
                |> Array.iter (fun x ->
                    x
                    |> textProcessing
                    |> printColor (Some ConsoleColor.Blue))
            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1

