module ArgParserTests

open UniqueFileGenerator.Console.ArgValidation
open UniqueFileGenerator.Console.ArgValidation.Types
open Xunit

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

// [<Fact>]
// let ``Success when valid file count`` () =
//     let args = [| "1000"; |]
//     let actual = validate args
//     let expected = Ok { FileCount = 1000; Options = defaultOptions }
//     printfn "%A" actual
//     printfn "%A" expected
//     Assert.Equal(actual, expected)

