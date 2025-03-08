namespace UniqueFileGenerator.Console

open System
open Errors
open Utilities
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

        type RawOptionPairs = Map<string,string>

        let flags: Map<OptionType, string> =
            [ Prefix, "-p"
              NameBaseLength, "-b"
              Extension, "-e"
              OutputDirectory, "-o"
              Size, "-s"
              Delay, "-d" ]
            |> Map.ofList

    open Types

    let supportedSeparators = [ ","; "_" ]

    let defaultOptions =
        { Prefix = String.Empty
          NameBaseLength = 50
          Extension = String.Empty
          OutputDirectory = "output"
          Size = None
          Delay = 0 }

    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error NoArgsPassed
        | l when isEven l -> Error ArgCountInvalid
        | _ -> Ok ()

    let private verifyFileCount (floor, ceiling) arg =
        arg
        |> stripSubStrings supportedSeparators
        |> parseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> InvalidNumber (arg, floor, ceiling))

    let private toPairs argPairs =
        argPairs
        |> Array.chunkBySize 2 // Will throw if array length is odd!
        |> Array.map (fun x -> x[0], x[1])
        |> Map.ofArray

    let private parseAndMapInvalidNumberError (floor, ceiling) (arg: string) =
        arg
        |> parseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> InvalidNumber (arg, floor, ceiling))

    let private parseBaseLength (floor, ceiling) optionPairs =
        optionPairs
        |> Map.tryFind flags[NameBaseLength]
        |> Option.map (stripSubStrings supportedSeparators)
        |> Option.map (fun arg -> arg |> parseAndMapInvalidNumberError (floor, ceiling))
        |> Option.defaultValue (Ok defaultOptions.NameBaseLength)

    let private parseSize (floor, ceiling) optionPairs =
        optionPairs
        |> Map.tryFind flags[Size]
        |> Option.map (stripSubStrings supportedSeparators)
        |> Option.map (fun arg -> arg |> parseAndMapInvalidNumberError (floor, ceiling))
        |> function
            | Some (Ok i) -> Ok (Some i)
            | Some (Error e) -> Error e
            | None -> Ok None

    let private parseDelay  (floor, ceiling) optionPairs =
        optionPairs
        |> Map.tryFind flags[Delay]
        |> Option.map (stripSubStrings supportedSeparators)
        |> Option.map (fun arg -> arg |> parseAndMapInvalidNumberError (floor, ceiling))
        |> Option.defaultValue (Ok defaultOptions.Delay)

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

    let private toOptions (optionMap : RawOptionPairs) baseLength size delay =
        let extractValue option fallback map =
            map
            |> Map.tryFind option
            |> Option.defaultValue fallback

        Ok {
            Prefix          = optionMap |> extractValue flags[Prefix] defaultOptions.Prefix
            NameBaseLength  = baseLength
            Extension       = optionMap |> extractValue flags[Extension] defaultOptions.Extension
            OutputDirectory = optionMap |> extractValue flags[OutputDirectory] defaultOptions.OutputDirectory
            Size            = size
            Delay           = delay
        }

    let validate args =
        result {
            do! verifyArgCount args
            let fileCountArg, optionArgs = args[0], args[1..]

            let! fileCount = verifyFileCount (1, Int32.MaxValue) fileCountArg

            let optionPairs = optionArgs |> toPairs
            do! verifyFlags optionPairs
            let! b = parseBaseLength (1, 100) optionPairs // Help avoid filename-length errors.
            let! s = parseSize (1, Int32.MaxValue) optionPairs
            let! d = parseDelay (0, Int32.MaxValue) optionPairs
            let! options = toOptions optionPairs b s d

            return { FileCount = fileCount
                     Options = options }
        }
