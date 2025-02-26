namespace UniqueFileGenerator.Console

module Io =
    open ArgValidation.Types
    open Errors
    open Printing
    open StringGenerator
    open System.IO
    open System.Threading

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    let verifyDirectory dir =
        match Directory.Exists dir with
        | true -> Ok dir
        | false -> Error (DirectoryMissing dir)

    let private createFile directory fileName (contents: string) =
        try
            let path = Path.Combine(directory, fileName)
            File.WriteAllText(path, contents)
            Ok fileName
        with
            | e -> Error $"%s{e.Message}"

    let generateFiles args =
        generateMultiple args.Options.NameBaseLength args.FileCount
            |> Array.map (fun text ->
                text |> modifyFileName args.Options.Prefix args.Options.Extension)
            |> Array.iter (fun text ->
                generateContent args.Options.Size text
                |> createFile args.Options.OutputDirectory text
                |> sleep args.Options.Delay
                |> printResult)
