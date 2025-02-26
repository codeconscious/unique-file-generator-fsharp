namespace UniqueFileGenerator.Console

open System
open FsToolkit.ErrorHandling

module ArgValidation =
    module Types =
        type OptionType =
            | Prefix
            | NameBaseLength
            | Extension
            | OutputDirectory
            | Size
            | Delay

        type Options =
            { Prefix: string
              NameBaseLength: int
              Extension: string
              OutputDirectory: string
              Size: int option
              Delay: int }

        type Args =
            { FileCount: int
              Options: Options }

        let flags: Map<OptionType, string> =
            [ (Prefix, "-p")
              (NameBaseLength, "-b")
              (Extension, "-e")
              (OutputDirectory, "-o")
              (Size, "-s")
              (Delay, "-d") ]
            |> Map.ofList

        type ValidationErrors =
            | NoArgsPassed
            | ArgCountInvalid
            | FileCountInvalid of string
            | MalformedFlags
            | UnsupportedFlags
            | DirectoryMissing of string

    open Types

    let private empty = String.Empty

    let defaultOptions =
        { Prefix = empty
          NameBaseLength = 50
          Extension = empty
          OutputDirectory = "output"
          Size = None
          Delay = 0 }

    let stripSeparators separators text =
        separators
        |> List.fold (fun (acc: string) s -> acc.Replace(s, empty)) text

    let private tryParseInt (input: string) =
        let strippedArg = input |> stripSeparators [ ","; "_" ]
        match Int32.TryParse(strippedArg) with
        | true, i -> Some i
        | false, _ -> None

    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error NoArgsPassed
        | l when isEven l -> Error ArgCountInvalid
        | _ -> Ok args

    let private verifyFileCount (args: string array) =
        let rawArg = Array.head args
        let strippedArg = rawArg |> stripSeparators [ ","; "_" ]

        match tryParseInt strippedArg with
        | Some c ->
            if c > 0 then Ok c
            else Error (FileCountInvalid rawArg)
        | None -> Error (FileCountInvalid rawArg)

    let private parseOptions options =
        let ensureBetween (floor, ceiling) i =
            i |> max floor |> min ceiling

        let hasMalformedOption optionPairs =
            let isCorrectFormat (o: string) =
                o.Length = 2 &&
                o.StartsWith("-") &&
                Char.IsLetter o[1]

            optionPairs
            |> Seq.forall isCorrectFormat
            |> not

        let extractValue option fallback map =
            map
            |> Map.tryFind option
            |> Option.defaultValue fallback

        let hasUnsupportedOption options =
            let isUnsupported (o: string) =
                flags
                |> Map.values
                |> Seq.contains o
                |> not

            options
            |> Seq.exists isUnsupported

        let optionMap =
            options
            |> Array.tail // Disregard the file count.
            |> Array.chunkBySize 2 // Will throw if array length is odd!
            |> Array.map (fun x -> (x[0], x[1]))
            |> Map.ofArray

        match optionMap with
        | o when o.Keys |> hasMalformedOption ->
            Error MalformedFlags
        | o when o.Keys |> hasUnsupportedOption ->
            Error UnsupportedFlags
        | o ->
            Ok {
                Prefix =          o |> extractValue flags[Prefix] defaultOptions.Prefix
                NameBaseLength =  o |> extractValue flags[NameBaseLength] empty
                                    |> tryParseInt
                                    |> Option.defaultValue defaultOptions.NameBaseLength
                                    |> ensureBetween (1, 100)
                Extension =       o |> extractValue flags[Extension] defaultOptions.Extension
                                    |> (fun x -> if x.StartsWith '.' then x[1..] else x)
                OutputDirectory = o |> extractValue flags[OutputDirectory] defaultOptions.OutputDirectory
                Size =            o |> extractValue flags[Size] empty
                                    |> tryParseInt
                Delay =           o |> extractValue flags[Delay] empty
                                    |> tryParseInt
                                    |> Option.defaultValue defaultOptions.Delay
            }

    let validate (args: string array) =
        result {
            let! args' = verifyArgCount args
            let! fileCount = verifyFileCount args'
            let! options = parseOptions args'
            return { FileCount = fileCount; Options = options }
        }
