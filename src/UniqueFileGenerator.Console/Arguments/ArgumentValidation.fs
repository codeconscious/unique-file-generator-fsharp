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

    let private verifyOptionArgs (optionPairs: Map<string, string>) =
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

            let! count = FileCount.Create fileCountArg

            let! optionArgPairs = optionArgs |> toPairs
            do! verifyOptionArgs optionArgPairs
            let tryGetArg x = optionArgPairs |> Map.tryFind flags[x]

            let p = Prefix.Create (tryGetArg Prefix)
            let! b = NameBaseLength.TryCreate (tryGetArg NameBaseLength)
            let e = Extension.Create (tryGetArg Extension)
            let o = OutputDirectory.Create (tryGetArg OutputDirectory)
            let! s = Size.TryCreate (tryGetArg Size)
            let! d = Delay.TryCreate (tryGetArg Delay)

            let options =
                { Prefix = p.Value
                  NameBaseLength = b.Value
                  Extension = e.Value
                  OutputDirectory = o.Value
                  Size = s.Value
                  Delay = d.Value }

            return Args.Create(count, options)
        }
