namespace UniqueFileGenerator.Console

open System
open System.Threading
open FsToolkit.ErrorHandling

module Main =
    open ArgValidation
    open ArgValidation.Types
    open Printing
    open StringGenerator
    open Io
    open Errors

    let private sleep (ms: int) x =
        Thread.Sleep ms
        x

    [<EntryPoint>]
    let main rawArgs =
        let watch = Startwatch.Library.Watch()

        let printResult = function
            | Ok x -> printLine $"OK: %s{x}"
            | Error e -> printError $"Error: %s{e}"

        let generateFiles args =
            generateMultiple args.Options.NameBaseLength args.FileCount
                |> Array.map (fun text ->
                    text |> modifyFileName args.Options.Prefix args.Options.Extension)
                |> Array.iter (fun text ->
                    generateContent args.Options.Size text
                    |> createFile args.Options.OutputDirectory text
                    |> sleep args.Options.Delay
                    |> printResult)

            printLine $"Done after %s{watch.ElapsedFriendly}"
            0

        let handleError e =
             e |> errorMessage |> printError
             Help.print()
             1

        let run (rawArgs: string array) =
            result {
                let! args = validate rawArgs
                let! _ = verifyDirectory args.Options.OutputDirectory
                return generateFiles args
            }

        match run rawArgs with
        | Ok exitCode -> exitCode
        | Error e -> handleError e
