namespace UniqueFileGenerator.Console

module Io =
    open Errors
    open Printing
    open ArgValidation.Types
    open StringGeneration
    open Utilities
    open System
    open System.IO
    open System.Threading

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    let private formatBytes (bytes: int64) : string =
        let kilobyte = 1024L
        let megabyte = kilobyte * 1024L
        let gigabyte = megabyte * 1024L
        let terabyte = gigabyte * 1024L

        match bytes with
        | _ when bytes >= terabyte -> sprintf "%s TB" ((float bytes / float terabyte) |> formatFloat)
        | _ when bytes >= gigabyte -> sprintf "%s GB" ((float bytes / float gigabyte) |> formatFloat)
        | _ when bytes >= megabyte -> sprintf "%s MB" ((float bytes / float megabyte) |> formatFloat)
        | _ when bytes >= kilobyte -> sprintf "%s KB" ((float bytes / float kilobyte) |> formatFloat)
        | _ -> sprintf "%s bytes" (bytes |> formatInt64)

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
                // printfn $"Needed: %d{necessaryDriveSpace}"
                // printfn $"Actual: %d{usableFreeSpace}"

                if necessaryDriveSpace < usableFreeSpace
                then Ok <| formatBytes necessaryDriveSpace
                else Error <| DriveSpaceInsufficient (formatBytes necessaryDriveSpace,
                                                      formatBytes usableFreeSpace)
        with
            | e -> Error <| UnknownError $"%s{e.Message}"

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
