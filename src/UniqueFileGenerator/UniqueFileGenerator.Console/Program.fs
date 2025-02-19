namespace UniqueFileGenerator.Console

open System
open System.Threading

module IO =
    open System.IO

    let createFile directory fileName (contents: string) =
        try
            let path = Path.Combine(directory, fileName)
            File.WriteAllText(path, contents)
            Ok fileName
        with
            | e -> Error $"%s{e.Message}"

module Main =
    open ArgValidation
    open Printing
    open StringGenerator
    open IO

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    [<EntryPoint>]
    let main args =
        let watch = Startwatch.Library.Watch()

        let printResult = function
            | Ok x -> printLine $"OK: %s{x}"
            | Error e -> printError $"Error: %s{e}"

        match validate args with
        | Ok a ->
            generateMultiple a.Options.NameBaseLength a.FileCount
                |> Array.map (fun text -> text |> modifyFileName a.Options.Prefix a.Options.Extension)
                |> Array.iter (fun text ->
                    generateContent a.Options.Size text
                    |> createFile a.Options.OutputDirectory text
                    |> sleep a.Options.Delay
                    |> printResult)

            printLine $"Done after %s{watch.ElapsedFriendly}"
            0
        | Error e ->
            let msg =
                match e with
                | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
                | ArgCountInvalid -> "Invalid argument count."
                | FileCountInvalid c -> $"Invalid file count: %s{c}."
                | MalformedFlags -> "Malformed flag(s) found."
                | UnsupportedFlags -> "Unsupported flag(s) found."
                | DirectoryMissing d -> $"Directory \"%s{d}\" does not exist."

            printError msg
            Help.print()
            1

