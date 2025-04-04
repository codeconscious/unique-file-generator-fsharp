namespace UniqueFileGenerator.Console

open System

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

    let toFileName (prefix: string) (extension: string) (baseName: string) : string =
        let safeExtension =
            match extension.Trim() with
            | ext when String.IsNullOrWhiteSpace ext -> String.Empty
            | ext when ext.StartsWith '.' -> ext
            | ext -> $".%s{ext}"

        $"%s{prefix}%s{baseName}%s{safeExtension}"

    let generateFileContent sizeInBytes fallback : string =
        sizeInBytes
        |> Option.map generateSingle
        |> Option.defaultValue fallback
