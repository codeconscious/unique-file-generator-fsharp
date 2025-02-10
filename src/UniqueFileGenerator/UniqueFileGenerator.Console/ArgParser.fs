namespace UniqueFileGenerator.Console

open System

module ArgValidation =
    type ParsedArgs = { FileCount: uint; Flags: Map<string, string> }

    let private supportedFlags =
        [
            "-p"; // Prefix
            "-e"; // Extension
            "-o"; // Output directory
            "-s"; // Size
            "-d"  // Delay
        ]

    type ResultBuilder() =
        member this.Bind(m, f) =
            match m with
            | Error e -> Error e
            | Ok a -> f a

        member this.Return(x) =
            Ok x

    let result = ResultBuilder()

    let private validate args =
        let hasMalformedFlag (flags: string seq) =
            not(flags |> Seq.forall (fun f -> f[0] = '-'))
        let hasUnsupportedFlag (flags: string seq) =
            flags |> Seq.exists (fun f -> not(supportedFlags |> List.contains f))

        match args with
        | a when a.FileCount < 1u ->
            Error "File count must be one or greater."
        | a when a.Flags.Keys |> hasMalformedFlag ->
            Error "Malformed flag(s) found. Each flag must start with a single hyphen (\"-\")."
        | a when a.Flags.Keys |> hasUnsupportedFlag ->
            Error "Unsupported flag(s) found."
        | _ -> Ok args

    let private isEven i =
        i % 2 = 0

    let private toPairs arr =
        arr
        |> Array.chunkBySize 2
        |> Array.map (fun x -> (x[0], x[1]))

    let private tryParseNumber (input: string) =
        match UInt32.TryParse(input.Replace(", ", String.Empty)) with
        | true, value -> Ok value
        | false, _ -> Error "File count must be one or greater."

    let startValidation (args: string array) =
        result {
            let! args' =
                match args.Length with
                | 0 -> Error "You must pass in at least one argument."
                | l when isEven l -> Error "Invalid argument count!"
                | _ -> Ok args

            let! count = args' |> Array.head |> tryParseNumber
            let args'' = {
                FileCount = count
                Flags = args' |> Array.tail |> toPairs |> Map.ofArray // TODO: Should break if dupes.
            }

            let! args''' = validate args''
            return args'''
        }
