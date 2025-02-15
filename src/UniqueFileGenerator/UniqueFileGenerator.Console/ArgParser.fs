namespace UniqueFileGenerator.Console

open System

module ArgValidation =
    type Options =
        { Prefix: string option
          Extension: string option
          OutputDirectory: string
          Size: int option
          Delay: int option }

    type Args =
        { FileCount: int
          Options: Options }

    let prefixFlag = "-p"
    let extensionFlag = "-e"
    let outputDirectoryFlag = "-o"
    let sizeFlag = "-s"
    let delayFlag = "-d"

    let defaultOutputDirectory = "output"

    let supportedFlags =
        [ prefixFlag
          extensionFlag
          outputDirectoryFlag
          sizeFlag
          delayFlag ]

    type ResultBuilder() =
        member this.Bind(m, f) =
            match m with
            | Error e -> Error e
            | Ok a -> f a

        member this.Return(x) =
            Ok x

    let result = ResultBuilder()

    let private tryParseInt (input: string) =
        match Int32.TryParse(input.Replace(", ", String.Empty)) with
        | true, i -> Some i
        | false, _ -> None

    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error "You must pass in at least one argument: the number of files to generate."
        | l when isEven l -> Error "Invalid argument count."
        | _ -> Ok args

    let private verifyFileCount (args: string array) =
        let countArg = args |> Array.head
        match tryParseInt countArg with
        | None -> Error $"Invalid file count: %s{countArg}."
        | Some c -> Ok c

    let private parseOptions (options: string array) =
        let hasMalformedOption (optionPairs: string seq) =
            let isCorrectFormat (o: string) =
                o.Length = 2 &&
                o.StartsWith("-") &&
                Char.IsLetter o[1]

            optionPairs
            |> Seq.forall isCorrectFormat
            |> not

        let extractValue option map =
            map
            |> Map.tryFind option

        let hasUnsupportedOption (options: string seq) =
            let isUnsupported (o: string) =
                supportedFlags
                |> List.contains o
                |> not

            options
            |> Seq.exists isUnsupported

        let optionMap =
            options
            |> Array.tail // Ignore the file count.
            |> Array.chunkBySize 2 // Will throw if array length is odd!
            |> Array.map (fun x -> (x[0], x[1]))
            |> Map.ofArray

        match optionMap with
        | o when o.Keys |> hasMalformedOption ->
            Error "Malformed flag(s) found."
        | o when o.Keys |> hasUnsupportedOption ->
            Error "Unsupported flag(s) found."
        | o ->
            Ok {
                Prefix =          o |> extractValue prefixFlag
                Extension =       o |> extractValue extensionFlag
                OutputDirectory = match o |> extractValue outputDirectoryFlag with
                                  | Some d -> d
                                  | None -> defaultOutputDirectory
                Size =            o |> extractValue sizeFlag
                                    |> Option.bind tryParseInt
                Delay =           o |> extractValue delayFlag
                                    |> Option.bind tryParseInt
            }

    let confirmDirectoryExists dir =
        match System.IO.Directory.Exists(dir) with
        | true -> Ok dir
        | false -> Error $"Directory \"%s{dir}\" does not exist."

    let validate (args: string array) =
        result {
            let! args' = verifyArgCount args
            let! fileCount = verifyFileCount args'
            let! options = parseOptions args'
            let! _ = confirmDirectoryExists options.OutputDirectory
            return { FileCount = fileCount; Options = options }
        }
