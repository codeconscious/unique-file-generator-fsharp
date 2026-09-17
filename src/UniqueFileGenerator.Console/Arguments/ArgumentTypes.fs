namespace UniqueFileGenerator.Console

open Errors
open Utilities
open System
open FSharpPlus
open CCFSharpUtils.Text

module ArgTypes =

    let supportedSeparators = [ ","; "_" ]

    let stripSeparatorsAndTrim =
        String.stripSubstrings supportedSeparators >> String.trim

    let private tryParseIntInRange (floor, ceiling) text =
        text
        |> tryParseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> NumberParseFailure (text, (floor, ceiling)))

    type FileCount = private FileCount of int with
        static member val AllowedRange = 1, Int32.MaxValue

        static member Create text : Result<FileCount, AppError> =
            text
            |> stripSeparatorsAndTrim
            |> tryParseInRange FileCount.AllowedRange
            |> Result.bimap
                (fun _ -> NumberParseFailure (text, FileCount.AllowedRange))
                FileCount

        member this.Value = let (FileCount count) = this in count

    type Prefix = private Prefix of string with
        static member val Default = String.Empty

        static member Create text =
            text
            |> option id Prefix.Default
            |> Prefix

        member this.Value = let (Prefix prefix) = this in prefix

    type NameBaseLength = private NameBaseLength of int with
        static member val AllowedRange = 1, 100
        static member val Default = 50

        static member TryCreate text =
            text
            |> option
                (stripSeparatorsAndTrim >> tryParseIntInRange NameBaseLength.AllowedRange)
                (Ok NameBaseLength.Default)
            |> Result.map NameBaseLength

        member this.Value = let (NameBaseLength length) = this in length

    type Extension = private Extension of string with
        static member val Default = String.Empty

        static member Create text =
            text
            |> option String.trim Extension.Default
            |> Extension

        member this.Value = let (Extension extension) = this in extension

    type OutputDirectory = private OutputDirectory of string with
        static member val Default = "output"

        static member Create text =
            text
            |> option String.trim OutputDirectory.Default
            |> OutputDirectory

        member this.Value = let (OutputDirectory dir) = this in dir

    type Size = private Size of int option with
        static member val AllowedRange = 1, Int32.MaxValue

        static member TryCreate text =
            text
            |> Option.map (stripSeparatorsAndTrim >> tryParseIntInRange Size.AllowedRange)
            |> function
               | Some (Ok i)    -> Ok (Size (Some i))
               | Some (Error e) -> Error e // Parse error.
               | None           -> Ok (Size None) // No size entered.

        member this.Value = let (Size size) = this in size

    type Delay = private Delay of int with
        static member val AllowedRange = 0, Int32.MaxValue
        static member val Default = 0

        static member TryCreate text =
            text
            |> option
                (stripSeparatorsAndTrim >> tryParseIntInRange Delay.AllowedRange)
                (Ok Delay.Default)
            |> Result.map Delay

        member this.Value = let (Delay length) = this in length

    type AppOption =
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

    let flags: Map<AppOption, string> =
        [ Prefix, "-p"
          NameBaseLength, "-b"
          Extension, "-e"
          OutputDirectory, "-o"
          Size, "-s"
          Delay, "-d" ]
        |> Map.ofList

    let fileNameLength options =
        (options.Prefix.Length + options.NameBaseLength + options.Extension.Length)
