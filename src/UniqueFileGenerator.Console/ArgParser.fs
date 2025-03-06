namespace UniqueFileGenerator.Console

open System
open Errors
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

        type MappedRawOptions = Map<string,string>

        let flags: Map<OptionType, string> =
            [ (Prefix, "-p")
              (NameBaseLength, "-b")
              (Extension, "-e")
              (OutputDirectory, "-o")
              (Size, "-s")
              (Delay, "-d") ]
            |> Map.ofList

    open Types

    let defaultOptions =
        { Prefix = String.Empty
          NameBaseLength = 50
          Extension = String.Empty
          OutputDirectory = "output"
          Size = None
          Delay = 0 }

    let supportedSeparators = [ ","; "_" ]

    let stripSeparators separators text =
        separators
        |> List.fold (fun (acc: string) s -> acc.Replace(s, String.Empty)) text

    let private tryParseInt (input: string) =
        let strippedArg = input |> stripSeparators supportedSeparators
        match Int32.TryParse strippedArg with
        | true, i -> Some i
        | false, _ -> None

    let private parseInRange (floor, ceiling) (x: string) =
        let error = InvalidNumber (x, floor, ceiling)
        match tryParseInt x with
        | None -> Error error
        | Some i ->
            if i < floor then Error error
            elif i > ceiling then Error error
            else Ok i

    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error NoArgsPassed
        | l when isEven l -> Error ArgCountInvalid
        | _ -> Ok args

    let private verifyFileCount (args: string array) =
        let rawArg = Array.head args
        let strippedArg = rawArg |> stripSeparators supportedSeparators

        match tryParseInt strippedArg with
        | Some c ->
            if c > 0 then Ok c
            else Error (FileCountInvalid rawArg)
        | None -> Error (FileCountInvalid rawArg)

    let private toOptionMap args =
        args
        |> Array.chunkBySize 2 // Will throw if array length is odd!
        |> Array.map (fun x -> x[0], x[1])
        |> Map.ofArray
    let private verifyBaseLength (optionMap : MappedRawOptions) =
        optionMap
        |> Map.tryFind flags[NameBaseLength]
        |> function
            | None -> Ok defaultOptions.NameBaseLength
            | Some x -> x |> parseInRange (1, Int32.MaxValue)

    let private verifySize (optionMap : MappedRawOptions) =
        optionMap
        |> Map.tryFind flags[Size]
        |> function
            | None -> Ok None
            | Some x ->
                match (x |> parseInRange (1, Int32.MaxValue)) with
                | Error e -> Error e
                | Ok i -> Ok (Some i)

    let private verifyDelay (optionMap : MappedRawOptions) =
        optionMap
        |> Map.tryFind flags[Delay]
        |> function
            | None -> Ok defaultOptions.Delay
            | Some x -> x |> parseInRange (0, Int32.MaxValue)

    let private verifyFlags (optionMap : MappedRawOptions) =
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

        match optionMap with
        | o when o.Keys |> hasMalformedOption -> Error MalformedFlags
        | o when o.Keys |> hasUnsupportedOption -> Error UnsupportedFlags
        | _ -> Ok ()

    let private parseOptions (optionMap : MappedRawOptions) baseLength size delay =
        let extractValue option fallback map =
            map
            |> Map.tryFind option
            |> Option.defaultValue fallback

        Ok {
            Prefix =          optionMap |> extractValue flags[Prefix] defaultOptions.Prefix
            NameBaseLength =  baseLength
            Extension =       optionMap |> extractValue flags[Extension] defaultOptions.Extension
            OutputDirectory = optionMap |> extractValue flags[OutputDirectory] defaultOptions.OutputDirectory
            Size =            size
            Delay =           delay
        }

    let validate (args: string array) =
        result {
            let! args' = verifyArgCount args
            let! fileCount = verifyFileCount args'

            let optionMap =
                args'
                |> Array.tail // Disregard the file count (in the initial position).
                |> toOptionMap

            let! _ = verifyFlags optionMap
            let! b = verifyBaseLength optionMap
            let! s = verifySize optionMap
            let! d = verifyDelay optionMap

            let! options = parseOptions optionMap b s d
            return { FileCount = fileCount
                     Options = options }
        }
