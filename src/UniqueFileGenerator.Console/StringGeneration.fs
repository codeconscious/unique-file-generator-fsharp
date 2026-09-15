namespace UniqueFileGenerator.Console

open System
open FSharpPlus
open CCFSharpUtils.Text

module StringGeneration =
    type FileNameParts =
        { Prefix: string
          Base: string
          Ext: string }

    let private charBank =
        [ 'A' .. 'Z' ] @ [ 'a' .. 'z' ] @ [ '0' .. '9' ]
        |> map string
        |> String.concat String.Empty

    let private rnd = Random()

    let private generateSingle length : string =
        String(Array.init length (fun _ -> charBank[rnd.Next(charBank.Length)]))

    let generateMultiple eachLength count : string array =
        Array.init count (fun _ -> generateSingle eachLength)

    // TODO: Consider returning an actual file object.
    let toFileName parts : string =
        let sanitizedExtension =
            match parts.Ext.Trim() with
            | ext when String.hasNoText ext -> String.Empty
            | ext when ext.StartsWith '.' -> ext
            | ext -> $".%s{ext}"

        String.Concat(
            parts.Prefix.Trim(),
            parts.Base,
            sanitizedExtension)

    let generateFileContent sizeInBytes fallback : string =
        sizeInBytes |> option generateSingle fallback
