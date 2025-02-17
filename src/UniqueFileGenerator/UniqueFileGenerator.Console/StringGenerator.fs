namespace UniqueFileGenerator.Console

open System
open System.Text

module StringGenerator =
    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> List.map string
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

    // let generateGuids count : string array =
    //     [| 0..count-1 |]
    //     |> Array.map (fun _ -> Guid.NewGuid().ToString())

    let modifyFileName prependText extension baseName =
        let prepend pre =
            fun fileName -> $"%s{pre}%s{fileName}"

        let appendExt ext =
            fun fileName -> $"%s{fileName}.%s{ext}"

        baseName
        |> prepend prependText
        |> appendExt extension

    let generateContent sizeInBytes fallback =
        match sizeInBytes with
        | None -> fallback
        | Some s -> generateSingle s
