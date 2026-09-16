namespace UniqueFileGenerator.Console

open UniqueFileGenerator.Console
open System
open Errors
open CCFSharpUtils
open FsToolkit.ErrorHandling
open ArgTypes

module ArgValidation =
    let private validateArgCount (args: string array) =
        match args.Length with
        | 0 -> Error NoArgsPassed
        | l when Num.isEven l -> Error ArgCountInvalid
        | _ -> Ok ()

    let private toPairs (args: string array) =
        let hasDuplicate xs =
            let originalLength = Seq.length xs
            let uniqueLength = xs |> Set.ofSeq |> Set.count
            originalLength <> uniqueLength

        args
        |> Array.chunkBySize 2 // Will throw if array length is odd!
        |> Array.map (fun x -> x[0].ToLowerInvariant(), x[1])
        |> fun pairs ->
            if pairs |> Array.map fst |> hasDuplicate
            then Error DuplicateFlags
            else Ok (Map.ofArray pairs)

    let private validateOptionArgs (optionPairs: Map<string, string>) =
        let hasMalformedOption optionPairs =
            let isCorrectFormat (o: string) =
                o.Length = 2 && o.StartsWith "-" && Char.IsLetter o[1]

            optionPairs
            |> Seq.forall isCorrectFormat
            |> not

        let hasUnknownOption appOptions =
            let isUnknown appOption = flags |> Map.values |> Seq.contains appOption |> not
            appOptions |> Seq.exists isUnknown

        match optionPairs.Keys with
        | keys when hasMalformedOption keys -> Error MalformedFlags
        | keys when hasUnknownOption keys   -> Error UnknownFlags
        | _ -> Ok ()

    let validate args =
        result {
            do! validateArgCount args
            let fileCountArg, optionArgs = args[0], args[1..]

            let! fileCount = FileCount.Create fileCountArg

            let! optionArgPairs = toPairs optionArgs
            do! validateOptionArgs optionArgPairs

            let tryGetArg x = Map.tryFind flags[x] optionArgPairs
            let  p = Prefix.Create (tryGetArg Prefix)
            let! b = NameBaseLength.TryCreate (tryGetArg NameBaseLength)
            let  e = Extension.Create (tryGetArg Extension)
            let  o = OutputDirectory.Create (tryGetArg OutputDirectory)
            let! s = Size.TryCreate (tryGetArg Size)
            let! d = Delay.TryCreate (tryGetArg Delay)

            let options =
                { Prefix = p.Value
                  NameBaseLength = b.Value
                  Extension = e.Value
                  OutputDirectory = o.Value
                  Size = s.Value
                  Delay = d.Value }

            return Args.Create(fileCount, options)
        }
