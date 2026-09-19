namespace UniqueFileGenerator.Console

open System
open FSharpPlus
open CCFSharpUtils
open CCFSharpUtils.Text

module StringGeneration =
    type FileNameParts =
        { Prefix: string; Base: string; Ext: string }

    let private charBank = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"

    let private rnd = Random.Shared

    let private generateSingle (length: int) : string =
        let sb = SB length
        List.init length (fun _ -> sb.Append charBank[rnd.Next charBank.Length]) |> ignore
        sb.ToString()

    let generateMultiple eachLength count : string list =
        List.init count (fun _ -> generateSingle eachLength)

    // TODO: Consider returning an actual file object.
    let toFileName parts : string =
        let sanitizedExtension =
            match parts.Ext.Trim() with
            | ext when String.hasNoText ext -> String.Empty
            | ext when ext.StartsWith '.' -> ext
            | ext -> $".{ext}"

        String.Concat(
            parts.Prefix.Trim(),
            parts.Base,
            sanitizedExtension)

    let generateFileContent sizeInBytes fallback : string =
        sizeInBytes |> option generateSingle fallback
