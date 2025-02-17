namespace UniqueFileGenerator.Console

open System

module ArgValidation =
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

    let private prefixFlag = "-p"
    let private nameBaseLengthFlag = "-b"
    let private extensionFlag = "-e"
    let private outputDirectoryFlag = "-o"
    let private sizeFlag = "-s"
    let private delayFlag = "-d"

    let private defaultOutputDirectory = "output"
    let private defaultNameBaseLengthCount = 256

    let private supportedFlags =
        [ prefixFlag
          nameBaseLengthFlag
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

    let private result = ResultBuilder()

    let private tryParseInt (input: string) =
        match Int32.TryParse(input.Replace(", ", String.Empty)) with
        | true, i -> Some i
        | false, _ -> None

    let private ensureBetween (floor, ceiling) i =
        i |> max floor |> min ceiling

    let private verifyArgCount (args: string array) =
        let isEven i = i % 2 = 0

        match args.Length with
        | 0 -> Error "You must pass in at least one argument: the number of files to generate."
        | l when isEven l -> Error "Invalid argument count."
        | _ -> Ok args

    let private verifyFileCount (args: string array) =
        let stripSeparators separators text =
            separators
            |> List.fold (fun (x: string) s -> x.Replace(s, String.Empty)) text

        let countArg = Array.head args |> stripSeparators [ ","; "_" ]

        match tryParseInt countArg with
        | None -> Error $"Invalid file count: {(Array.head args)}."
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

        let extractValue option fallback map =
            map
            |> Map.tryFind option
            |> Option.defaultValue fallback

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
                Prefix =          o |> extractValue prefixFlag String.Empty
                NameBaseLength =  o |> extractValue nameBaseLengthFlag String.Empty
                                    |> tryParseInt
                                    |> Option.defaultValue defaultNameBaseLengthCount
                                    |> ensureBetween (1, 100)
                Extension =       o |> extractValue extensionFlag String.Empty
                                    |> (fun x -> if x[0] = '.' then x[1..] else x)
                OutputDirectory = o |> extractValue outputDirectoryFlag defaultOutputDirectory
                Size =            o |> extractValue sizeFlag String.Empty
                                    |> tryParseInt
                Delay =           o |> extractValue delayFlag String.Empty
                                    |> tryParseInt
                                    |> Option.defaultValue 0
            }

    let private verifyDirectory options =
        let dir = options.OutputDirectory
        match IO.Directory.Exists dir with
        | true -> Ok options
        | false -> Error $"Directory \"%s{dir}\" does not exist."

    let validate (args: string array) =
        result {
            let! args' = verifyArgCount args
            let! fileCount = verifyFileCount args'
            let! options = parseOptions args'
            let! options' = verifyDirectory options
            return { FileCount = fileCount; Options = options' }
        }
