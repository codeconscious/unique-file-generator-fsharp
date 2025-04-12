namespace UniqueFileGenerator.Console

open System

module StringGeneration =
    type FileNameParts =
        { Prefix: string
          Base: string
          Ext: string }

    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> List.map string
        |> String.concat String.Empty

    let private rnd = Random()

    let private generate length : string =
        String(Array.init length (fun _ -> charBank[rnd.Next(charBank.Length)]))

    let generateMultiple eachLength count : string array =
        Array.init count (fun _ -> generate eachLength)

    let toFileName parts : string =
        let sanitizedExtension =
            match parts.Ext.Trim() with
            | ext when String.IsNullOrWhiteSpace ext -> String.Empty
            | ext when ext.StartsWith '.' -> ext
            | ext -> $".%s{ext}"

        $"%s{parts.Prefix.Trim()}%s{parts.Base}%s{sanitizedExtension}"

    let generateFileContent sizeInBytes fallback : string =
        sizeInBytes
        |> Option.map generate
        |> Option.defaultValue fallback
