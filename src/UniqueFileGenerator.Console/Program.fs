namespace UniqueFileGenerator.Console

open ArgValidation
open ArgTypes
open Errors
open Printing
open Io
open FsToolkit.ErrorHandling

module Main =
    [<EntryPoint>]
    let main rawArgs =
        let watch = Startwatch.Library.Watch()

        let run (rawArgs: string array) =
            result {
                let! args = validate rawArgs
                do! verifyDirectory args.Options.OutputDirectory
                let! spaceNeeded = verifyDriveSpace args

                generateFiles args
                return spaceNeeded
            }

        if Help.wasRequested rawArgs then
            Help.print ()
            0
        else
            match run rawArgs with
            | Ok spaceUsed ->
                printLine $"Done after %s{watch.ElapsedFriendly}. Used approximately %s{spaceUsed} of drive space."
                0
            | Error e ->
                printError <| getMessage e
                Help.suggest ()
                1
