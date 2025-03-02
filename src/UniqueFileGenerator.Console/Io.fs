namespace UniqueFileGenerator.Console

module Io =
    open Errors
    open Printing
    open ArgValidation.Types
    open StringGeneration
    open System
    open System.IO
    open System.Threading

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    let verifyDriveSpace (args: Args) =
        let necessaryDriveSpace =
            let o = args.Options

            let singleFileSize =
                o.Size
                |> Option.defaultValue (o.Prefix.Length + o.NameBaseLength + o.Extension.Length)
                |> int64

            printfn $"Math: %d{args.FileCount} files * %d{singleFileSize} bytes"
            int64 args.FileCount * singleFileSize // Rough estimation

        let spaceToKeepAvailable = 536_870_912L // 0.5 GB

        try
            let appDir = AppContext.BaseDirectory
            let root = Path.GetPathRoot appDir

            match root with
            | null -> Error DriveSpaceConfirmationFailure
            | path ->
                let driveInfo = DriveInfo path
                let usableFreeSpace = driveInfo.AvailableFreeSpace - spaceToKeepAvailable
                printfn $"Needed: %d{necessaryDriveSpace}"
                printfn $"Actual: %d{usableFreeSpace}"

                if necessaryDriveSpace < usableFreeSpace
                then Ok necessaryDriveSpace
                else Error <| InsufficientDriveSpace (necessaryDriveSpace, usableFreeSpace)
        with
            | e -> Error $"%s{e.Message}"

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

        let updateFileName baseName =
            baseName
            |> toFileName prefix extension

        let writeFile fileName =
            fileName
            |> generateFileContent size
            |> createFile outputDir fileName
            |> sleep delay
            |> printResult

        generateMultiple baseLength count
            |> Array.map updateFileName
            |> Array.iter writeFile
