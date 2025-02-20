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
    let main rawArgs =
        let watch = Startwatch.Library.Watch()

        let printResult = function
            | Ok x -> printLine $"OK: %s{x}"
            | Error e -> printError $"Error: %s{e}"

        let generateFiles args =
            generateMultiple args.Options.NameBaseLength args.FileCount
                |> Array.map (fun text ->
                    text |> modifyFileName args.Options.Prefix args.Options.Extension)
                |> Array.iter (fun text ->
                    generateContent args.Options.Size text
                    |> createFile args.Options.OutputDirectory text
                    |> sleep args.Options.Delay
                    |> printResult)

            printLine $"Done after %s{watch.ElapsedFriendly}"
            0

        let printValidationErrors e =
            let msg =
                match e with
                | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
                | ArgCountInvalid -> "Invalid argument count."
                | FileCountInvalid c -> $"Invalid file count: %s{c}."
                | MalformedFlags -> "Malformed flag(s) found."
                | UnsupportedFlags -> "Unsupported flag(s) found."
                | DirectoryMissing d -> $"Directory \"%s{d}\" does not exist."

            printError msg
            Help.print ()
            1

        match validate rawArgs with
        | Ok args -> generateFiles args
        | Error e -> printValidationErrors e

