namespace UniqueFileGenerator.Console

open ArgTypes
open Errors
open System
open CCFSharpUtils
open FsToolkit.ErrorHandling

module ArgValidation =
    /// Ensure the count of args is odd, which is currently the only valid shape.
    let private validateArgCount (args: string array) =
        match args.Length with
        | 0 -> Error ArgsMissing
        | l when Num.isEven l -> Error ArgCountInvalid
        | _ -> Ok ()

    let private validateOptionArgs (optionMap: Map<string, string>) =
        let hasMalformedOptionKey keys =
            let isCorrectFormat (o: string) =
                o.Length = 2 && o.StartsWith "-" && Char.IsLetter o[1]

            keys |> Seq.forall isCorrectFormat |> not

        let hasUnknownOptionKey appOptions =
            let isUnknown appOption = flags |> Map.values |> Seq.contains appOption |> not
            appOptions |> Seq.exists isUnknown

        match optionMap.Keys with
        | keys when hasMalformedOptionKey keys -> Error MalformedFlags
        | keys when hasUnknownOptionKey keys   -> Error UnknownFlags
        | _ -> Ok optionMap

    let private toPairs (args: string array) =
        let hasDuplicate xs =
            let originalLength = Seq.length xs
            let uniqueLength = xs |> Set.ofSeq |> Set.count
            originalLength <> uniqueLength

        args
        |> Array.chunkBySize 2
        |> Array.map (fun pair -> pair[0].ToLowerInvariant(), pair[1])
        |> fun pairs ->
            // Check for duplicates here because conversion to a map will
            // silently use only the last duplicate (though that is apparently
            // undocumented behavior with a chance of changing in the future).
            if hasDuplicate (Array.map fst pairs)
            then Error DuplicateFlags
            else validateOptionArgs (Map.ofArray pairs)

    let validate args =
        result {
            do! validateArgCount args

            let! fileCount = FileCount.TryCreate (Array.head args)
            let! optionMap = toPairs (Array.tail args)

            let tryGetArg x = Map.tryFind flags[x] optionMap
            let! p = Prefix.Create (tryGetArg Prefix)
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
