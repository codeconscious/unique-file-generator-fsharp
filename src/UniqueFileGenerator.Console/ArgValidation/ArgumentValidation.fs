namespace UniqueFileGenerator.Console

open UniqueFileGenerator.Console
open System
open Errors
open FsToolkit.ErrorHandling
open ArgTypes

module ArgValidation =
    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error NoArgsPassed
        | l when isEven l -> Error ArgCountInvalid
        | _ -> Ok ()

    let private toPairs argPairs =
        argPairs
        |> Array.chunkBySize 2 // Will throw if array length is odd!
        |> Array.map (fun x -> x[0], x[1])
        |> Map.ofArray

    let private verifyFlags (optionPairs: RawOptionPairs) =
        let hasMalformedOption optionPairs =
            let isCorrectFormat (o: string) =
                o.Length = 2 &&
                o.StartsWith "-" &&
                Char.IsLetter o[1]

            optionPairs
            |> Seq.forall isCorrectFormat
            |> not

        let hasUnsupportedOption options =
            let isUnsupported option =
                flags
                |> Map.values
                |> Seq.contains option
                |> not

            options
            |> Seq.exists isUnsupported

        match optionPairs with
        | o when o.Keys |> hasMalformedOption -> Error MalformedFlags
        | o when o.Keys |> hasUnsupportedOption -> Error UnsupportedFlags
        | _ -> Ok ()

    let validate args =
        result {
            do! verifyArgCount args
            let fileCountArg, optionArgs = args[0], args[1..]

            let! fileCount = FileCount.Create fileCountArg

            let optionPairs = optionArgs |> toPairs
            do! verifyFlags optionPairs

            let readArg x = optionPairs |> Map.tryFind flags[x]
            let p = Prefix.Create (readArg Prefix)
            let! b = NameBaseLength.TryCreate (readArg NameBaseLength)
            let e = Extension.Create (readArg Extension)
            let o = OutputDirectory.Create (readArg OutputDirectory)
            let! s = Size.TryCreate (readArg Size)
            let! d = Delay.TryCreate (readArg Delay)

            return {
                FileCount = fileCount
                Options = {
                     Prefix = p.Value
                     NameBaseLength = b.Value
                     Extension = e.Value
                     OutputDirectory = o.Value
                     Size = s.Value
                     Delay = d.Value
                }
            }
        }
