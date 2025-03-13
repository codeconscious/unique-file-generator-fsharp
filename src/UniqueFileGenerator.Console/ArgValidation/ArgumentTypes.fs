namespace UniqueFileGenerator.Console

open Errors
open Utilities
open System
open FsToolkit.ErrorHandling

module ArgTypes =
    let stripSeparators text : string =
        let supportedSeparators = [ ","; "_" ]

        supportedSeparators
        |> List.fold (fun acc s -> acc.Replace(s, String.Empty)) text

    let private tryParseIntInRange (floor, ceiling) (text: string) =
        text
        |> parseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> InvalidNumber (text, floor, ceiling))

    type FileCount = private FileCount of int with
        static member val AllowedRange = 1, Int32.MaxValue

        static member Create (text: string) =
            text
            |> stripSeparators
            |> parseInRange FileCount.AllowedRange
            |> Result.mapError (fun _ ->
                InvalidNumber (text,
                               fst FileCount.AllowedRange,
                               snd FileCount.AllowedRange))
            |> Result.map FileCount

        member this.Value = let (FileCount count) = this in count

    type Prefix = private Prefix of string with
        static member val Default = String.Empty

        static member Create (text: string option) =
            match text with
            | None -> Prefix.Default
            | Some x -> x
            |> Prefix

        member this.Value = let (Prefix prefix) = this in prefix

    type NameBaseLength = private NameBaseLength of int with
        static member val AllowedRange = 1, 100
        static member val Default = 50

        static member TryCreate (text: string option) =
            text
            |> Option.map stripSeparators
            |> Option.map (fun arg -> arg |> tryParseIntInRange NameBaseLength.AllowedRange)
            |> Option.defaultValue (Ok NameBaseLength.Default)
            |> Result.map NameBaseLength

        member this.Value = let (NameBaseLength length) = this in length

    type Extension = private Extension of string with
        static member val Default = String.Empty

        static member Create (text: string option) =
            match text with
            | None -> Extension.Default
            | Some x -> x.Trim()
            |> Extension

        member this.Value = let (Extension extension) = this in extension

    type OutputDirectory = private OutputDirectory of string with
        static member val Default = "output"

        static member Create (text: string option) =
            match text with
            | None -> OutputDirectory.Default
            | Some x -> x.Trim()
            |> OutputDirectory

        member this.Value = let (OutputDirectory dir) = this in dir

    type Size = private Size of int option with
        static member val AllowedRange = 1, Int32.MaxValue

        static member TryCreate (text: string option) =
            text
            |> Option.map stripSeparators
            |> Option.map (fun arg -> arg.Trim() |> tryParseIntInRange Size.AllowedRange)
            |> function
               | Some (Error e) -> Error e
               | Some (Ok i) -> Ok (Size (Some i))
               | None -> Ok (Size None)

        member this.Value = let (Size size) = this in size

    type Delay = private Delay of int with
        static member val AllowedRange = 0, Int32.MaxValue
        static member val Default = 0

        static member TryCreate (text: string option) =
            text
            |> Option.map stripSeparators
            |> Option.map (fun arg -> arg.Trim() |> tryParseIntInRange Delay.AllowedRange)
            |> Option.defaultValue (Ok Delay.Default)
            |> Result.map Delay

        member this.Value = let (Delay length) = this in length

    type OptionType =
        | Prefix
        | NameBaseLength
        | Extension
        | OutputDirectory
        | Size
        | Delay

    type Options =
        { Prefix: string
          NameBaseLength: int
          Extension: string
          OutputDirectory: string
          Size: int option
          Delay: int }

    type Args =
        { FileCount: int
          Options: Options }

    type GetArgs = private GetArgs of Args with
        static member Create (count: FileCount, p: Prefix, b: NameBaseLength,
                              e: Extension, o: OutputDirectory, s: Size, d: Delay) =
            { FileCount = count.Value
              Options =
                { Prefix = p.Value
                  NameBaseLength = b.Value
                  Extension = e.Value
                  OutputDirectory = o.Value
                  Size = s.Value
                  Delay = d.Value }
            }

    let flags: Map<OptionType, string> =
        [ Prefix, "-p"
          NameBaseLength, "-b"
          Extension, "-e"
          OutputDirectory, "-o"
          Size, "-s"
          Delay, "-d" ]
        |> Map.ofList
