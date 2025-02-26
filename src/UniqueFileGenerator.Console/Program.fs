namespace UniqueFileGenerator.Console

open FsToolkit.ErrorHandling

module Main =
    open ArgValidation
    open ArgValidation.Types
    open Printing
    open Io
    open Errors

    [<EntryPoint>]
    let main rawArgs =
        let watch = Startwatch.Library.Watch()

        let run (rawArgs: string array) =
            result {
                let! args = validate rawArgs
                let! _ = verifyDirectory args.Options.OutputDirectory
                return generateFiles args
            }

        match run rawArgs with
        | Ok _ ->
            printLine $"Done after %s{watch.ElapsedFriendly}"
            0
        | Error e ->
             e |> errorMessage |> printError
             Help.print()
             1
