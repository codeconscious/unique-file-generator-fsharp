namespace UniqueFileGenerator.Console

open System
open System.Text

module StringGeneration =
    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> List.map string
        |> String.concat String.Empty

    let private generateSingle length : string =
        let rnd = Random()
        let getRndChar () = charBank[rnd.Next charBank.Length]
        let chars = Array.init length (fun _ -> getRndChar ())
        new string(chars)

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
