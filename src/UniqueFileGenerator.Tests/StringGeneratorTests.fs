module StringGeneratorTests

open System
open UniqueFileGenerator.Console.StringGenerator
open Xunit

module Generation =
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
