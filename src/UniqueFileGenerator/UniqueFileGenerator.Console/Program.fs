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

module IO =
    open System.IO

    // let directoryExists dir =
    //     match Directory.Exists(dir) with
    //     | true -> Ok dir
    //     | false -> Error $"Directory \"{dir}\" does not exist."

    let createFile directory fileName (contents: string) =
        match directory with
        | None -> Error "A directory must be supplied."
        | Some d ->
            try
                let path = Path.Combine(d, fileName)
                File.WriteAllText(path, contents)
                Ok fileName
            with
                | e -> Error $"%s{e.Message}"

module Main =
    open ArgValidation
    open Printing
    open IO

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
        let validatedArgs = validate args

        match validatedArgs with
        | Ok a ->
            let ext = appendExt a
            let pre = prepend a
            let processText = ext >> pre

            StringGenerator.generateMultiple 10 a.FileCount
                |> Array.map (fun x -> x |> processText)
                |> Array.map (fun f -> createFile a.Options.OutputDirectory f "content")
                |> Array.iter (fun x ->
                    match x with
                    | Ok f -> $"OK: %s{f}" |> printColor None
                    | Error e -> $"Error: %s{e}" |> printColor (Some(ConsoleColor.Red)))

            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1

