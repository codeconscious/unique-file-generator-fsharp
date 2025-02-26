module ArgParserTests

open UniqueFileGenerator.Console.ArgValidation
open UniqueFileGenerator.Console.ArgValidation.Types
open Xunit

let testValues =
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
    let emptyArgs = [| "12"; "-p" |]
    let actual = validate emptyArgs
    let expected = Error ArgCountInvalid
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when invalid arg count (second pair incomplete)`` () =
    let emptyArgs = [| "12"; "-p"; "__"; "-e" |]
    let actual = validate emptyArgs
    let expected = Error ArgCountInvalid
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when invalid file count`` () =
    let args = [| "invalid" |]
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
    let args = [| "1000"; "malformedFlagWithNoHyphen"; "0" |]
    let actual = validate args
    let expected = Error MalformedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when unsupported symbol flag found`` () =
    let unsupportedFlag = "-@"
    let args = [| "1000"; unsupportedFlag; "0" |]
    let actual = validate args
    let expected = Error MalformedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Appropriate error when unsupported flag(s) found`` () =
    let unsupportedFlag = "-a"
    let args = [| "1000"; unsupportedFlag; "0" |]
    let actual = validate args
    let expected = Error UnsupportedFlags
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count`` () =
    let args = [| "1000"; |]
    let actual = validate args
    let expected = Ok { FileCount = 1000; Options = defaultOptions }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix`` () =
    let args = [| "1000"; "-p"; testValues[Prefix] |]
    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with Prefix = testValues[Prefix] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix and extension`` () =
    let args = [| "1000"; "-p"; testValues[Prefix]; "-e"; testValues[Extension] |]
    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with
                        Prefix = testValues[Prefix]
                        Extension = testValues[Extension][1..] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, and base length`` () =
    let args = [|
        "1000"
        "-p"; testValues[Prefix]
        "-e"; testValues[Extension]
        "-b"; testValues[NameBaseLength]
    |]

    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with
                        Prefix = testValues[Prefix]
                        Extension = testValues[Extension][1..]
                        NameBaseLength = int testValues[NameBaseLength] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, and custom subdirectory`` () =
    let args = [|
        "1000"
        "-p"; testValues[Prefix]
        "-e"; testValues[Extension]
        "-b"; testValues[NameBaseLength]
        "-o"; testValues[OutputDirectory]
    |]

    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with
                        Prefix = testValues[Prefix]
                        Extension = testValues[Extension][1..]
                        NameBaseLength = int testValues[NameBaseLength]
                        OutputDirectory = testValues[OutputDirectory] } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, custom subdirectory, and size`` () =
    let args = [|
        "1000"
        "-p"; testValues[Prefix]
        "-e"; testValues[Extension]
        "-b"; testValues[NameBaseLength]
        "-o"; testValues[OutputDirectory]
        "-s"; testValues[Size]
    |]
    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with
                        Prefix = testValues[Prefix]
                        Extension = testValues[Extension][1..]
                        NameBaseLength = int testValues[NameBaseLength]
                        OutputDirectory = testValues[OutputDirectory]
                        Size = Some (int testValues[Size]) } }
    Assert.Equal(actual, expected)

[<Fact>]
let ``Success when valid file count with prefix, extension, base length, custom subdirectory, size, and delay`` () =
    let args = [|
        "1000"
        "-p"; testValues[Prefix]
        "-e"; testValues[Extension]
        "-b"; testValues[NameBaseLength]
        "-o"; testValues[OutputDirectory]
        "-s"; testValues[Size]
        "-d"; string testValues[Delay]
    |]

    let actual = validate args
    let expected = Ok {
        FileCount = 1000
        Options = { defaultOptions with
                        Prefix = testValues[Prefix]
                        Extension = testValues[Extension][1..]
                        NameBaseLength = int testValues[NameBaseLength]
                        OutputDirectory = testValues[OutputDirectory]
                        Size = Some (int testValues[Size])
                        Delay = int testValues[Delay] } }
    Assert.Equal(actual, expected)
