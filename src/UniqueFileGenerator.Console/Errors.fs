namespace UniqueFileGenerator.Console

open System
open System.Globalization

module Errors =

    let private formatInt (i: int) =
        i.ToString("N0", CultureInfo.InvariantCulture)

    type ErrorType =
        | NoArgsPassed
        | ArgCountInvalid
        | FileCountInvalid of string
        | MalformedFlags
        | UnsupportedFlags
        | UnparsableToPositiveInt of string
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | DriveSpaceInsufficient of Needed: string * Actual: string
        | UnknownError of string

    let getMessage error =
        match error with
        | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid argument count."
        | FileCountInvalid c -> $"Invalid file count: %s{c}."
        | MalformedFlags -> "Malformed flag(s) found."
        | UnsupportedFlags -> "Unsupported flag(s) found."
        | UnparsableToPositiveInt x -> $"Could not parse \"%s{x}\" to an integer between 1 and %s{formatInt Int32.MaxValue}."
        | DirectoryMissing e -> $"Directory \"%s{e}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space. %s{needed} is necessary, but only %s{actual} is available."
        | UnknownError e -> e

