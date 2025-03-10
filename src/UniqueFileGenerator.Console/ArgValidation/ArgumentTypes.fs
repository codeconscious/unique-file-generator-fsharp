namespace UniqueFileGenerator.Console

open System
open Errors
open Utilities
open FsToolkit.ErrorHandling

module ArgTypes =
    let supportedSeparators = [ ","; "_" ]

    let private parseNumberInRange (floor, ceiling) (arg: string) =
        arg
        |> parseInRange (floor, ceiling)
        |> Result.mapError (fun _ -> InvalidNumber (arg, floor, ceiling))

    type FileCount = private FileCount of int with
        static member val AllowedRange = 1, Int32.MaxValue

        static member Create (input: string) =
            input
            |> stripSubStrings supportedSeparators
            |> parseInRange (fst FileCount.AllowedRange,
                             snd FileCount.AllowedRange)
            |> Result.mapError (fun _ ->
                InvalidNumber (input,
                               fst FileCount.AllowedRange,
                               snd FileCount.AllowedRange))
            |> Result.map FileCount

        member this.Value = let (FileCount count) = this in count

    type Prefix = private Prefix of string with
        static member val DefaultValue = String.Empty

        static member Create (input: string option) =
            match input with
            | None -> Prefix.DefaultValue
            | Some x -> x
            |> Prefix

        member this.Value = let (Prefix prefix) = this in prefix

    type NameBaseLength = private NameBaseLength of int with
        static member val DefaultValue = 50

        static member TryCreate (input: string option) =
            let allowedRange = 1, 100

            input
            |> Option.map (stripSubStrings supportedSeparators)
            |> Option.map (fun arg ->
                arg
                |> parseNumberInRange allowedRange)
            |> Option.defaultValue (Ok NameBaseLength.DefaultValue)
            |> Result.map NameBaseLength

        member this.Value = let (NameBaseLength length) = this in length

    type Extension = private Extension of string with
        static member val DefaultValue = String.Empty

        static member Create (input: string option) =
            match input with
            | None -> Extension.DefaultValue
            | Some x -> x.Trim()
            |> Extension

        member this.Value = let (Extension extension) = this in extension

    type OutputDirectory = private OutputDirectory of string with
        static member val DefaultValue = "output"

        static member Create (input: string option) =
            match input with
            | None -> OutputDirectory.DefaultValue
            | Some x -> x.Trim()
            |> OutputDirectory

        member this.Value = let (OutputDirectory dir) = this in dir

    type Size = private Size of int option with
      static member val AllowedRange = 1, Int32.MaxValue

      static member TryCreate (input: string option) =
          input
          |> Option.map (stripSubStrings supportedSeparators)
          |> Option.map (fun arg -> arg |> parseNumberInRange Size.AllowedRange)
          |> function
              | Some (Ok i) -> Ok (Size (Some i))
              | Some (Error e) -> Error e
              | None -> Ok (Size None)

      member this.Value = let (Size length) = this in length

    type Delay = private Delay of int with
        static member val AllowedRange = 0, Int32.MaxValue
        static member val DefaultValue = 0

        static member TryCreate (input: string option) =
            input
            |> Option.map (stripSubStrings supportedSeparators)
            |> Option.map (fun arg -> arg |> parseNumberInRange Delay.AllowedRange)
            |> Option.defaultValue (Ok Delay.DefaultValue)
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

    type RawOptionPairs = Map<string,string>

    let flags: Map<OptionType, string> =
        [ Prefix, "-p"
          NameBaseLength, "-b"
          Extension, "-e"
          OutputDirectory, "-o"
          Size, "-s"
          Delay, "-d" ]
        |> Map.ofList
