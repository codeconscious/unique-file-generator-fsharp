﻿namespace UniqueFileGenerator.Console

open ArgValidation
open ArgValidation.Types
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
                let! _ = verifyDirectory args.Options.OutputDirectory
                return generateFiles args
            }

        if Help.wasRequested rawArgs then
            Help.print ()
            0
        else
            match run rawArgs with
            | Ok _ ->
                printLine $"Done after %s{watch.ElapsedFriendly}"
                0
            | Error e ->
                printError <| getMessage e
                Help.suggest ()
                1
