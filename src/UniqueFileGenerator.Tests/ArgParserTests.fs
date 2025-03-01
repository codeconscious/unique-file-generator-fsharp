module ArgParserTests

open UniqueFileGenerator.Console.ArgValidation
open UniqueFileGenerator.Console.ArgValidation.Types
open UniqueFileGenerator.Console.Errors
open Xunit

let validFileCountArg = "1000"

let validOptionValues =
    Map.ofList<OptionType, string>
        [ Prefix, "PREFIX_"
          NameBaseLength, "60"
          Extension, ".txt"
          OutputDirectory, "何らかのフォルダー名"
          Size, "1_000_000"
          Delay, "5_000" ]

[<Fact>]
let ``Appropriate error when no args`` () =
    let emptyArgs = [||]
    let actual = validate emptyArgs
    let expected = Error NoArgsPassed
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when invalid arg count (first pair incomplete)`` () =
    let emptyArgs = [| "12"; flags[Prefix] |]
    let actual = validate emptyArgs
    let expected = Error ArgCountInvalid
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when invalid arg count (second pair incomplete)`` () =
    let emptyArgs = [| "12"; flags[Prefix]; "__"; flags[Extension] |]
    let actual = validate emptyArgs
    let expected = Error ArgCountInvalid
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when invalid file count`` () =
    let args = [| "notNumeric" |]
    let actual = validate args
    let expected = Error <| FileCountInvalid(args[0])
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when negative file count`` () =
    let args = [| "-1" |]
    let actual = validate args
    let expected = Error <| FileCountInvalid(args[0])
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when zero file count`` () =
    let args = [| "0" |]
    let actual = validate args
    let expected = Error <| FileCountInvalid(args[0])
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when malformed flags found`` () =
    let args = [| validFileCountArg; "malformedFlagWithNoHyphen"; "0" |]
    let actual = validate args
    let expected = Error MalformedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when unsupported symbol flag found`` () =
    let unsupportedFlag = "-@"
    let args = [| validFileCountArg; unsupportedFlag; "0" |]
    let actual = validate args
    let expected = Error MalformedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when unsupported flag(s) found`` () =
    let unsupportedFlag = "-a"
    let args = [| validFileCountArg; unsupportedFlag; "0" |]
    let actual = validate args
    let expected = Error UnsupportedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count`` () =
    let args = [| validFileCountArg; |]
    let actual = validate args
    let expected = Ok { FileCount = int validFileCountArg; Options = defaultOptions }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
    |]
    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with Prefix = validOptionValues[Prefix] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix and extension (with initial period)`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
    |]
    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix and extension (without initial period)`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension][1..]
    |]
    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension][1..] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, and base length`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
    |]

    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension]
                        NameBaseLength = int validOptionValues[NameBaseLength] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, and custom subdirectory`` () =
    let args = [|
        validFileCountArg
        flags[Prefix]; validOptionValues[Prefix]
        flags[Extension]; validOptionValues[Extension]
        flags[NameBaseLength]; validOptionValues[NameBaseLength]
        flags[OutputDirectory]; validOptionValues[OutputDirectory]
    |]

    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension]
                        NameBaseLength = int validOptionValues[NameBaseLength]
                        OutputDirectory = validOptionValues[OutputDirectory] } }
    Assert.Equal(actual, expected)

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
    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension]
                        NameBaseLength = int validOptionValues[NameBaseLength]
                        OutputDirectory = validOptionValues[OutputDirectory]
                        Size = Some (int validOptionValues[Size]) } }
    Assert.Equal(actual, expected)

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

    let actual = validate args
    let expected = Ok {
        FileCount = int validFileCountArg
        Options = { defaultOptions with
                        Prefix = validOptionValues[Prefix]
                        Extension = validOptionValues[Extension]
                        NameBaseLength = int validOptionValues[NameBaseLength]
                        OutputDirectory = validOptionValues[OutputDirectory]
                        Size = Some (int validOptionValues[Size])
                        Delay = int validOptionValues[Delay] } }
    Assert.Equal(actual, expected)
