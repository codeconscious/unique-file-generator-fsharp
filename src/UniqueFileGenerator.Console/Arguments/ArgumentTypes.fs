namespace UniqueFileGenerator.Console

open Errors
open Utilities
open System
open FsToolkit.ErrorHandling

module ArgTypes =
    let stripSeparators text : string =
        let supportedSeparators = [ ","; "_" ]

        (text, supportedSeparators)
        ||> List.fold (fun acc s -> acc.Replace(s, String.Empty))

    let private tryParseIntInRange (floor, ceiling) text =
        text
        |> parseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> ParseNumberFailure (text, floor, ceiling))

    type FileCount = private FileCount of int with
        static member val AllowedRange = 1, Int32.MaxValue

        static member Create (text: string) =
            text
            |> stripSeparators
            |> parseInRange FileCount.AllowedRange
            |> Result.mapError (fun _ ->
                ParseNumberFailure (text, fst FileCount.AllowedRange, snd FileCount.AllowedRange))
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
            |> Option.map (fun arg -> arg.Trim() |> tryParseIntInRange NameBaseLength.AllowedRange)
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
               | Some (Ok i) -> Ok (Size (Some i))
               | Some (Error e) -> Error e // Parse error.
               | None -> Ok (Size None) // No size entered.

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
        private
            { fileCount: int
              options: Options }

        member x.FileCount = x.fileCount
        member x.Options = x.options

        static member Create (count: FileCount, options: Options) =
            { fileCount = count.Value
              options =
                { Prefix = options.Prefix
                  NameBaseLength = options.NameBaseLength
                  Extension = options.Extension
                  OutputDirectory = options.OutputDirectory
                  Size = options.Size
                  Delay = options.Delay } }

    let flags: Map<OptionType, string> =
        [ Prefix, "-p"
          NameBaseLength, "-b"
          Extension, "-e"
          OutputDirectory, "-o"
          Size, "-s"
          Delay, "-d" ]
        |> Map.ofList

    let fileNameLength options =
        (options.Prefix.Length + options.NameBaseLength + options.Extension.Length)
