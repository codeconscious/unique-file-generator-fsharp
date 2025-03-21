module ArgParserTests

open System
open UniqueFileGenerator.Console.ArgValidation
open UniqueFileGenerator.Console.ArgTypes
open UniqueFileGenerator.Console.Errors
open Xunit

let validFileCountArg = "1000"

let validOptionValues =
    Map.ofList<OptionType, string>
        [ Prefix, "PREFIX "
          NameBaseLength, "60"
          Extension, ".txt"
          OutputDirectory, "何らかのフォルダー名"
          Size, "2_000_000"
          Delay, "5_000" ]

let defaultOptions =
    { Prefix = Prefix.Create None |> _.Value
      NameBaseLength =
          NameBaseLength.TryCreate None
          |> function
              | Ok x -> x.Value
              | Error e -> failwith $"Unexpected parse error: {e}"
      Extension = Extension.Create None |> _.Value
      OutputDirectory = OutputDirectory.Create None |> _.Value
      Size = Size.TryCreate None
             |> function
                 | Ok x -> x.Value
                 | Error e -> failwith $"Unexpected parse error: {e}"
      Delay = Delay.TryCreate None
              |> function
                  | Ok x -> x.Value
                  | Error e -> failwith $"Unexpected parse error: {e}"}

let createOkArgs fileCount options =
    match FileCount.Create fileCount with
    | Ok fc -> Ok <| Args.Create(fc, options)
    | Error _ -> failwith "Unexpected error in test setup!"

[<Fact>]
let ``Appropriate error when no args`` () =
    let emptyArgs = [||]
    let expected = Error NoArgsPassed
    let actual = validate emptyArgs
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when invalid arg count (first pair incomplete)`` () =
    let emptyArgs = [| "12"; flags[Prefix] |]
    let expected = Error ArgCountInvalid
    let actual = validate emptyArgs
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when invalid arg count (second pair incomplete)`` () =
    let emptyArgs = [| "12"; flags[Prefix]; "__"; flags[Extension] |]
    let expected = Error ArgCountInvalid
    let actual = validate emptyArgs
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when invalid file count`` () =
    let args = [| "notNumeric" |]
    let expected = Error <| ParseNumberFailure(args[0], 1, Int32.MaxValue)
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when negative file count`` () =
    let args = [| "-1" |]
    let expected = Error <| ParseNumberFailure(args[0], 1, Int32.MaxValue)
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when zero file count`` () =
    let args = [| "0" |]
    let expected = Error <| ParseNumberFailure(args[0], 1, Int32.MaxValue)
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when malformed flags found`` () =
    let args = [| validFileCountArg; "malformedFlagWithNoHyphen"; "0" |]
    let expected = Error MalformedFlags
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when unsupported symbol flag found`` () =
    let unsupportedFlag = "-@"
    let args = [| validFileCountArg; unsupportedFlag; "0" |]
    let expected = Error MalformedFlags
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when unsupported flag(s) found`` () =
    let unsupportedFlag = "-a"
    let args = [| validFileCountArg; unsupportedFlag; "0" |]
    let expected = Error UnsupportedFlags
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count`` () =
    let args = [| validFileCountArg |]
    let expected = createOkArgs validFileCountArg defaultOptions
    let actual = validate args
    Assert.Equal(expected, actual)


[<Fact>]
let ``Success when valid file count with prefix`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
    |]
    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with Prefix = validOptionValues[Prefix] }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix and extension (with initial period)`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
    |]
    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension] }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix and extension (without initial period)`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension][1..]
    |]
    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension][1..] }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix, extension, and base length`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
    |]

    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension]
                NameBaseLength = int validOptionValues[NameBaseLength] }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, and custom subdirectory`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
        flags[OutputDirectory]; validOptionValues[OutputDirectory]
    |]

    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension]
                NameBaseLength = int validOptionValues[NameBaseLength]
                OutputDirectory = validOptionValues[OutputDirectory] }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, custom subdirectory, and size`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
        flags[OutputDirectory]; validOptionValues[OutputDirectory]
        flags[Size]; validOptionValues[Size]
    |]
    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension]
                NameBaseLength = int validOptionValues[NameBaseLength]
                OutputDirectory = validOptionValues[OutputDirectory]
                Size = Some (int validOptionValues[Size]) }
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, custom subdirectory, size, and delay`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
        flags[OutputDirectory]; validOptionValues[OutputDirectory]
        flags[Size]; validOptionValues[Size]
        flags[Delay]; string validOptionValues[Delay]
    |]

    let expected =
        createOkArgs
            validFileCountArg
            { defaultOptions with
                Prefix = validOptionValues[Prefix]
                Extension = validOptionValues[Extension]
                NameBaseLength = int validOptionValues[NameBaseLength]
                OutputDirectory = validOptionValues[OutputDirectory]
                Size = Some (int validOptionValues[Size])
                Delay = int validOptionValues[Delay] }
    let actual = validate args
    Assert.Equal(expected, actual)
