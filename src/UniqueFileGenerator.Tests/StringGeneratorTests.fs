module StringGeneratorTests

open System
open UniqueFileGenerator.Console.StringGenerator
open Xunit

module Generator =
    [<Fact>]
    let ``Generates strings with valid args`` () =
        let itemLength = 111
        let count = 222
        let generated = generateMultiple itemLength count

        Assert.True(generated |> Array.forall (fun x -> x.Length = itemLength))
        Assert.True(generated.Length = count)

    [<Fact>]
    let ``Generates nothing with 0 count`` () =
        let itemLength = 111
        let count = 0
        let generated = generateMultiple itemLength count

        Assert.Empty(generated)

    [<Fact>]
    let ``Generates empty strings with 0 length`` () =
        let itemLength = 0
        let count = 222
        let generated = generateMultiple itemLength count

        Assert.True(generated |> Array.forall (fun x -> x.Length = itemLength))
        Assert.True(generated.Length = count)

    [<Fact>]
    let ``Throws with negative count`` () =
        Assert.Throws<ArgumentException>(fun () ->
            (generateMultiple 5000 -1) :> obj)

    [<Fact>]
    let ``Throws with negative item length`` () =
        Assert.Throws<ArgumentOutOfRangeException>(fun () ->
            (generateMultiple -1 5000) :> obj)

module FileNames =
    [<Fact>]
    let ``Generates filenames without a prefix or extension when not provided`` () =
        let prefix = String.Empty
        let extension = String.Empty
        let generated = generateMultiple 10 10

        let fileNames =
            generated
            |> Array.map (fun x ->
                x |> updateFileName prefix extension)

        Assert.Equal<string[]>(generated, fileNames)

    [<Fact>]
    let ``Generates filenames with a prefix and no extension`` () =
        let prefix = "@@"
        let extension = String.Empty
        let generated = generateMultiple 10 10

        let fileNames =
            generated
            |> Array.map (fun x ->
                x |> updateFileName prefix extension)

        Assert.True(fileNames |> Array.forall (fun x -> x.StartsWith prefix))
        Assert.True(fileNames |> Array.forall (fun x -> not <| x.Contains '.'))

    [<Fact>]
    let ``Generates filenames with an extension and no prefix`` () =
        let prefix = String.Empty
        let extension = ".txt"
        let generated = generateMultiple 10 10

        let fileNames =
            generated
            |> Array.map (fun x ->
                x |> updateFileName prefix extension)

        Assert.True(fileNames |> Array.forall (fun x -> x.EndsWith extension))

    [<Fact>]
    let ``Generates filenames with a prefix and extension args`` () =
        let prefix = "@@"
        let extension = ".txt"
        let generated = generateMultiple 10 10

        let fileNames =
            generated
            |> Array.map (fun x ->
                x |> updateFileName prefix extension)

        Assert.True(fileNames |> Array.forall (fun x -> x.StartsWith prefix))
        Assert.True(fileNames |> Array.forall (fun x -> x.EndsWith extension))
