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
    open IO

    // TODO: Move elsewhere.
    let private prepend pre =
        fun fileName -> $"%s{pre}%s{fileName}"

    let private appendExt ext =
        fun fileName -> $"%s{fileName}.%s{ext}"

    let private generateContent sizeInBytes fallback =
        match sizeInBytes with
        | None -> fallback
        | Some s -> StringGenerator.generateSingle s

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    [<EntryPoint>]
    let main args =
        let watch = Startwatch.Library.Watch()
        let validatedArgs = validate args

        match validatedArgs with
        | Ok a ->
            let pre = prepend a.Options.Prefix
            let ext = appendExt a.Options.Extension
            let processText = ext >> pre

            StringGenerator.generateMultiple a.Options.NameBaseCharCount a.FileCount
                |> Array.map (fun x -> x |> processText)
                |> Array.iter (fun f ->
                    let content = generateContent a.Options.Size f

                    createFile a.Options.OutputDirectory f content
                    |> sleep a.Options.Delay
                    |> (fun r ->
                        match r with
                            | Ok f -> $"OK: %s{f}" |> printColor None
                            | Error e -> $"Error: %s{e}" |> printColor (Some(ConsoleColor.Red))))

            $"Done after %s{watch.ElapsedFriendly}" |> printColor None
            0
        | Error e ->
            e |> printColor (Some ConsoleColor.Red)
            Help.printInstructions() |> ignore
            1

