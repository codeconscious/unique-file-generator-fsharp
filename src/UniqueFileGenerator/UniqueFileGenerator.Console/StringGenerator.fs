namespace UniqueFileGenerator.Console

open System
open System.Text

module StringGenerator =
    // let private uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    // let private lowercase = "abcdefghijklmnopqrstuvwxyz"
    // let private numbers = "0123456789"
    // let private charBank = uppercase + lowercase + numbers
    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> Seq.map string
        |> String.concat String.Empty

    let private rnd = Random()

    let generateSingle (length: int) : string =
        [| 0..length |]
        |> Array.fold (fun (state: StringBuilder) i ->
            match i with
            | i when i = length -> state
            | _ ->
                let nextChar = rnd.Next(0, charBank.Length - 1)
                state.Append charBank[nextChar]) (StringBuilder(length))
        |> _.ToString()

    let generateMultiple eachLength count : string array =
        [| 0..count-1 |]
        |> Array.map (fun _ -> generateSingle eachLength)

    let generateGuids count : string array =
        [| 0..count-1 |]
        |> Array.map (fun _ -> Guid.NewGuid().ToString())
