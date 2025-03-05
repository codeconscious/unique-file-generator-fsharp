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

        let flags: Map<OptionType, string> =
            [ (Prefix, "-p")
              (NameBaseLength, "-b")
              (Extension, "-e")
              (OutputDirectory, "-o")
              (Size, "-s")
              (Delay, "-d") ]
            |> Map.ofList

        type ParsedIntError =
            | TooLow of string
            | TooHigh of string
            | NaN of string

    open Types

    let private empty = String.Empty

    let defaultOptions =
        { Prefix = empty
          NameBaseLength = 50
          Extension = empty
          OutputDirectory = "output"
          Size = None
          Delay = 0 }

    let supportedSeparators = [ ","; "_" ]

    let stripSeparators separators text =
        separators
        |> List.fold (fun (acc: string) s -> acc.Replace(s, empty)) text

    let private tryParseInt (input: string) =
        let strippedArg = input |> stripSeparators supportedSeparators
        match Int32.TryParse strippedArg with
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
        let strippedArg = rawArg |> stripSeparators supportedSeparators

        match tryParseInt strippedArg with
        | Some c ->
            if c > 0 then Ok c
            else Error (FileCountInvalid rawArg)
        | None -> Error (FileCountInvalid rawArg)

    let private parseOptions options =
        let parseInRange (floor, ceiling) (x: string) =
            let error = InvalidNumber (x, floor, ceiling)
            match tryParseInt x with
            | None -> Error error
            | Some i ->
                if i < floor then Error error
                elif i > ceiling then Error error
                else Ok i

        let hasMalformedOption optionPairs =
            let isCorrectFormat (o: string) =
                o.Length = 2 &&
                o.StartsWith "-" &&
                Char.IsLetter o[1]

            optionPairs
            |> Seq.forall isCorrectFormat
            |> not

        let extractValue option fallback map =
            map
            |> Map.tryFind option
            |> Option.defaultValue fallback

        let hasUnsupportedOption options =
            let isUnsupported option =
                flags
                |> Map.values
                |> Seq.contains option
                |> not

            options
            |> Seq.exists isUnsupported

        let optionMap =
            options
            |> Array.tail // Disregard the file count.
            |> Array.chunkBySize 2 // Will throw if array length is odd!
            |> Array.map (fun x -> x[0], x[1])
            |> Map.ofArray

        let nameBaseLength =
            optionMap
            |> Map.tryFind flags[NameBaseLength]
            |> function
                | None -> Ok defaultOptions.NameBaseLength
                | Some x -> x |> parseInRange (1, Int32.MaxValue)

        let size =
            optionMap
            |> Map.tryFind flags[Size]
            |> function
                | None -> Ok None
                | Some x ->
                    match (x |> parseInRange (1, Int32.MaxValue)) with
                    | Error e -> Error e
                    | Ok i -> Ok (Some i)

        match optionMap with
        | o when o.Keys |> hasMalformedOption ->
            Error MalformedFlags
        | o when o.Keys |> hasUnsupportedOption ->
            Error UnsupportedFlags
        | o ->
            match (nameBaseLength, size) with
            | Error e, _ -> Error e
            | _, Error e -> Error e
            | Ok b, Ok s ->
                Ok {
                    Prefix =          o |> extractValue flags[Prefix] defaultOptions.Prefix
                    NameBaseLength =  b
                    Extension =       o |> extractValue flags[Extension] defaultOptions.Extension
                    OutputDirectory = o |> extractValue flags[OutputDirectory] defaultOptions.OutputDirectory
                    Size =            s
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
