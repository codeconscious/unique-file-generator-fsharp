module ArgParserTests

open System
open UniqueFileGenerator.Console.ArgValidation
open UniqueFileGenerator.Console.ArgTypes
open UniqueFileGenerator.Console.Errors
open Xunit

let validFileCountArg = "1000"

let validOptionValues =
    Map.ofList<AppOption, string>
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
    let expected = Error ArgsMissing
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
    let expected = Error (NumberParseFailure(args[0], (1, Int32.MaxValue)))
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when negative file count`` () =
    let args = [| "-1" |]
    let expected = Error (NumberParseFailure(args[0], (1, Int32.MaxValue)))
    let actual = validate args
    Assert.Equal(expected, actual)

[<Fact>]
let ``Appropriate error when zero file count`` () =
    let args = [| "0" |]
    let expected = Error (NumberParseFailure(args[0], (1, Int32.MaxValue)))
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
    let expected = Error UnknownFlags
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

module SupportedSeparators =

    [<Fact>]
    let ``Strips commas from text`` () =
        let text = "hello,world"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld", result)

    [<Fact>]
    let ``Strips underscores from text`` () =
        let text = "hello_world"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld", result)

    [<Fact>]
    let ``Strips both commas and underscores and trims`` () =
        let text = "  hello,world_test  "
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworldtest", result)

    [<Fact>]
    let ``Strips multiple consecutive separators`` () =
        let text = "hello,,__world"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld", result)

    [<Fact>]
    let ``Returns empty string when input is only separators`` () =
        let text = ",_,_,"
        let result = stripSeparatorsAndTrim text

        Assert.Equal(String.Empty, result)

    [<Fact>]
    let ``Returns empty string for empty input`` () =
        let text = String.Empty
        let result = stripSeparatorsAndTrim text

        Assert.Equal(String.Empty, result)

    [<Fact>]
    let ``Returns unchanged text with no separators`` () =
        let text = "helloworld"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld", result)

    [<Fact>]
    let ``Preserves whitespace`` () =
        let text = "hello , world _ test"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("hello  world  test", result)

    [<Fact>]
    let ``Preserves other punctuation`` () =
        let text = "hello,world.test_example!done"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld.testexample!done", result)

    [<Fact>]
    let ``Preserves numbers`` () =
        let text = "test_123,456_abc"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("test123456abc", result)

    [<Fact>]
    let ``Handles mixed case correctly`` () =
        let text = "Hello,World_Test"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("HelloWorldTest", result)

    [<Fact>]
    let ``Handles Unicode characters`` () =
        let text = "café,naïve_résumé,東京と京都"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("cafénaïverésumé東京と京都", result)

    [<Fact>]
    let ``Handles single character input`` () =
        let text = "_"
        let result = stripSeparatorsAndTrim text

        Assert.Equal(String.Empty, result)

    [<Fact>]
    let ``Handles single character without separator`` () =
        let text = "a"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("a", result)

    [<Fact>]
    let ``Handles separators at start`` () =
        let text = "_,hello"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("hello", result)

    [<Fact>]
    let ``Handles separators at end`` () =
        let text = "hello_,"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("hello", result)

    [<Fact>]
    let ``Handles separators at both ends`` () =
        let text = ",_hello_,"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("hello", result)

    [<Fact>]
    let ``Handles large strings`` () =
        let text = String.replicate 1000 "a_b,"
        let result = stripSeparatorsAndTrim text

        Assert.Equal(String.replicate 1000 "ab", result)

    [<Fact>]
    let ``Handles tabs and newlines`` () =
        let text = "hello,world\ttest_example\nmore"
        let result = stripSeparatorsAndTrim text

        Assert.Equal("helloworld\ttestexample\nmore", result)

