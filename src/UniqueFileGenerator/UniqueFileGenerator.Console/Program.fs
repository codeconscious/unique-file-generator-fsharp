namespace UniqueFileGenerator.Console

open System
open System.Threading

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
            | Ok x -> $"OK: %s{x}" |> printColor None
            | Error e -> $"Error: %s{e}" |> printColor (Some(ConsoleColor.Red))

        match validate args with
        | Ok a ->
            generateMultiple a.Options.NameBaseCharCount a.FileCount
                |> Array.map (fun x ->
                    x |> modifyFileName a.Options.Prefix a.Options.Extension)
                |> Array.iter (fun x ->
                    generateContent a.Options.Size x
                    |> createFile a.Options.OutputDirectory x
                    |> sleep a.Options.Delay
                    |> printResult)

            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1

