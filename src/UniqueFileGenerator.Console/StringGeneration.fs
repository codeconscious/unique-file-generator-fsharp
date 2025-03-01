namespace UniqueFileGenerator.Console

open System
open System.Text

module StringGeneration =
    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> List.map string
        |> String.concat String.Empty

    let private rnd = Random()

    let private generateSingle (length: int) : string =
        let sb = StringBuilder length
        for _ in 1 .. length do
            let nextChar = rnd.Next(0, charBank.Length - 1)
            sb.Append charBank[nextChar] |> ignore
        sb.ToString()

    let generateMultiple itemLength count : string array =
        Array.init count (fun _ -> generateSingle itemLength)

    let toFileName prefix extension baseName : string =
        let ensureValidExtension ext =
            if String.IsNullOrWhiteSpace ext then String.Empty
            elif ext.StartsWith '.' then ext.Trim()
            else $".%s{ext.Trim()}"

        $"%s{prefix}%s{baseName}%s{ensureValidExtension extension}"

    let generateFileContent sizeInBytes fallback : string =
        sizeInBytes
        |> Option.map generateSingle
        |> Option.defaultValue fallback

