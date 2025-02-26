namespace UniqueFileGenerator.Console

open System
open System.Text

module StringGenerator =
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

    let generateMultiple eachLength count : string array =
        Array.init count (fun _ -> generateSingle eachLength)

    let updateFileName prependText extensionText baseName =
        let prepend fileName = $"%s{prependText}%s{fileName}"
        let appendExtension fileName = $"%s{fileName}.%s{extensionText}"

        baseName
        |> prepend
        |> appendExtension

    let generateFileContent sizeInBytes fallback =
        sizeInBytes
        |> Option.map generateSingle
        |> Option.defaultValue fallback

