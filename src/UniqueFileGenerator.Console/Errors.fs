namespace UniqueFileGenerator.Console

open Utilities

module Errors =
    type ErrorType =
        | NoArgsPassed
        | ArgCountInvalid
        | FileCountInvalid of string
        | MalformedFlags
        | UnsupportedFlags
        | InvalidNumber of string * int * int
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
        | InvalidNumber (x, f, c) -> $"Could not parse \"%s{x}\" to an integer between %s{formatInt f} and %s{formatInt c}."
        | DirectoryMissing e -> $"Directory \"%s{e}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space. Though %s{needed} is necessary, only %s{actual} is available."
        | UnknownError e -> e

