namespace UniqueFileGenerator.Console

open ArgTypes
open Errors
open Printing
open StringGeneration
open Utilities
open System
open System.IO
open System.Threading

module Io =
    let verifyDirectory dir =
        match Directory.Exists dir with
        | true -> Ok ()
        | false -> Error (DirectoryMissing dir)

    let private formatBytes (bytes: int64) =
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
        let driveSpaceToKeepAvailable = 536_870_912L // 0.5 GB
        let warningRatio = 0.75

        let necessarySpace =
            let singleFileSize =
                args.Options.Size
                |> Option.defaultValue (fileNameLength args.Options)
                |> int64

            singleFileSize * (int64 args.FileCount) // Rough estimation

        let confirmContinueDespiteLargeSize usableFreeSpace : bool =
            let ratio = float necessarySpace / float usableFreeSpace
            let isLargeRatio = ratio > warningRatio

            let confirm () =
                Console.Write(
                    sprintf "This operation requires %s, which is %s%% of remaining drive space. Continue? (Y/n)  "
                        (necessarySpace |> formatBytes)
                        (ratio * 100.0 |> formatFloat))

                let reply = Console.ReadLine().Trim()

                [| "y"; "yes" |]
                |> Array.exists (fun yesAnswer -> reply.Equals(yesAnswer, StringComparison.InvariantCultureIgnoreCase))

            if isLargeRatio
            then confirm ()
            else true

        try
            let appDir = AppContext.BaseDirectory
            let root = Path.GetPathRoot appDir

            match root with
            | null -> Error DriveSpaceConfirmationFailure
            | path ->
                let driveInfo = DriveInfo path
                let usableFreeSpace = driveInfo.AvailableFreeSpace - driveSpaceToKeepAvailable

                if necessarySpace > usableFreeSpace
                then Error (DriveSpaceInsufficient (formatBytes necessarySpace, formatBytes usableFreeSpace))
                elif confirmContinueDespiteLargeSize usableFreeSpace
                then Ok (formatBytes necessarySpace)
                else Error CancelledByUser
        with
            | e -> Error (IoError $"%s{e.Message}")

    let private createFile directory fileName (contents: string) =
        try
            let path = Path.Combine(directory, fileName)
            File.WriteAllText(path, contents)
            Ok fileName
        with
            | e -> Error $"%s{e.Message}"

    let generateFiles (args: Args) =
        let count, prefix, baseLength, ext, outputDir, size, delay =
            args.FileCount,
            args.Options.Prefix,
            args.Options.NameBaseLength,
            args.Options.Extension,
            args.Options.OutputDirectory,
            args.Options.Size,
            args.Options.Delay

        let generateFileName baseName =
            toFileName { Prefix = prefix; Base = baseName; Ext = ext }

        let sleep (ms: int) x =
            Thread.Sleep ms
            x

        let writeFile fileName =
            fileName
            |> generateFileContent size
            |> createFile outputDir fileName
            |> sleep delay
            |> printResult

        generateMultiple baseLength count
        |> Array.map generateFileName
        |> Array.iter writeFile
