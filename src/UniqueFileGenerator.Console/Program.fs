namespace UniqueFileGenerator.Console

open ArgValidation
open ArgTypes
open Errors
open Printing
open Io
open FsToolkit.ErrorHandling

module Main =

    type ExitCode =
        | Success = 0
        | Error = 1

    [<EntryPoint>]
    let main rawArgs =
        let watch = Startwatch.Library.Watch()

        let run rawArgs =
            result {
                let! args = validate rawArgs
                do! verifyDirectory args.Options.OutputDirectory
                let! spaceNeeded = verifyDriveSpace args

                generateFiles args
                return spaceNeeded
            }

        if Help.isRequested rawArgs then
            Help.print ()
            ExitCode.Success
        else
            match run rawArgs with
            | Ok spaceUsed ->
                printLine $"Done after %s{watch.ElapsedFriendly}. Used approximately %s{spaceUsed} of drive space."
                ExitCode.Success
            | Error e ->
                printError <| errorMsg e
                Help.suggest ()
                ExitCode.Error
        |> int
