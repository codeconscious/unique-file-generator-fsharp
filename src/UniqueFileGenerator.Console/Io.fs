namespace UniqueFileGenerator.Console

open ArgTypes
open Errors
open Printing
open StringGeneration
open Utilities
open System
open System.IO
open System.Threading
open CCFSharpUtils.Operators
open CCFSharpUtils.Text

module Io =
    let verifyDirectory dir =
        if Directory.Exists dir
        then Ok ()
        else Error (DirectoryMissing dir)

    let private formatBytes bytes =
        let kb = 1024L
        let mb = kb * 1024L
        let gb = mb * 1024L
        let tb = gb * 1024L

        match bytes with
        | _ when bytes >= tb -> sprintf "%s TB" ((float bytes / float tb) |> formatFloat)
        | _ when bytes >= gb -> sprintf "%s GB" ((float bytes / float gb) |> formatFloat)
        | _ when bytes >= mb -> sprintf "%s MB" ((float bytes / float mb) |> formatFloat)
        | _ when bytes >= kb -> sprintf "%s KB" ((float bytes / float kb) |> formatFloat)
        | _ -> sprintf "%s bytes" (bytes |> formatInt64)

    let verifyDriveSpace (args: Args) =
        let driveSpaceToKeepAvailable = 536_870_912L // 0.5 GB
        let warningRatio = 0.75

        let neededSpace =
            args.Options.Size
            |> Option.defaultValue (fileNameLength args.Options)
            |> int64
            |> (*) (int64 args.FileCount) // Rough estimation

        let confirmContinueDespiteLargeSize availableSpace : bool =
            let ratio = float neededSpace / float availableSpace
            let isLargeRatio = ratio > warningRatio
            let yesAnswers = [| "y"; "yes" |]

            let confirm () =
                Console.Write(
                    sprintf "This operation requires %s, which is %s%% of remaining drive space. Continue? (Y/n)  "
                        (neededSpace |> formatBytes)
                        (ratio * 100.0 |> formatFloat))

                let reply = Console.ReadLine().Trim()

                Array.exists (String.equalIgnoreCase reply) yesAnswers

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

                if neededSpace > usableFreeSpace
                then Error (DriveSpaceInsufficient (formatBytes neededSpace, formatBytes usableFreeSpace))
                elif confirmContinueDespiteLargeSize usableFreeSpace
                then Ok (formatBytes neededSpace)
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

    let generateFiles (args: Args) : unit =
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

        let writeFile fileName =
            fileName
            |> generateFileContent size
            |> createFile outputDir fileName
            |-- (fun _ -> Thread.Sleep delay)
            |> printResult

        generateMultiple baseLength count
        |> Array.map generateFileName
        |> Array.iter writeFile
