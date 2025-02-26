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
        let count, prefix, baseLength, extension, outputDir, size, delay =
            (args.FileCount,
             args.Options.Prefix,
             args.Options.NameBaseLength,
             args.Options.Extension,
             args.Options.OutputDirectory,
             args.Options.Size,
             args.Options.Delay)

        generateMultiple baseLength count
            |> Array.map (fun baseName -> baseName |> updateFileName prefix extension)
            |> Array.iter (fun fileName ->
                generateFileContent size fileName
                |> createFile outputDir fileName
                |> sleep delay
                |> printResult)
