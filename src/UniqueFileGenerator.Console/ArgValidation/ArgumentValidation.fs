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

    let private toPairs (argPairs: string array) =
        let hasDuplicate xs =
            let originalLength = Seq.length xs
            let uniqueLength = xs |> Set.ofSeq |> Set.count
            originalLength <> uniqueLength

        argPairs
        |> Array.chunkBySize 2 // Will throw if array length is odd!
        |> Array.map (fun x -> x[0].ToLowerInvariant(), x[1])
        |> fun pairs ->
            match pairs |> Array.map fst |> hasDuplicate with
            | true -> Error DuplicateFlags
            | false -> Ok (Map.ofArray pairs)

    let private verifyOptionFlags (optionPairs: Map<string, string>) =
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

            let! optionPairs = optionArgs |> toPairs
            do! verifyOptionFlags optionPairs

            let tryExtractArg x = optionPairs |> Map.tryFind flags[x]

            let p = Prefix.Create (tryExtractArg Prefix)
            let! b = NameBaseLength.TryCreate (tryExtractArg NameBaseLength)
            let e = Extension.Create (tryExtractArg Extension)
            let o = OutputDirectory.Create (tryExtractArg OutputDirectory)
            let! s = Size.TryCreate (tryExtractArg Size)
            let! d = Delay.TryCreate (tryExtractArg Delay)

            return {
                FileCount = fileCount.Value
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
