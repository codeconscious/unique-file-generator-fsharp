namespace UniqueFileGenerator.Console

open Utilities

module Errors =
    type ErrorType =
        | NoArgsPassed
        | ArgCountInvalid
        | MalformedFlags
        | UnsupportedFlags
        | DuplicateFlags
        | InvalidNumber of Arg: string * Floor: int * Ceiling: int
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | DriveSpaceInsufficient of Needed: string * Actual: string
        | CancelledByUser
        | UnknownError of string

    let getMessage error =
        match error with
        | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid argument count."
        | MalformedFlags -> "Malformed flag(s) found."
        | UnsupportedFlags -> "Unsupported flag(s) found."
        | DuplicateFlags -> "Duplicate option flag(s) found. Each can only be used once."
        | InvalidNumber (x, f, c) ->
            $"Could not parse \"%s{x}\" to an integer between %s{formatInt f} and %s{formatInt c}, inclusive."
        | DirectoryMissing e -> $"Directory \"%s{e}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space. Though %s{needed} is necessary, only %s{actual} is available."
        | CancelledByUser -> "Cancelled."
        | UnknownError e -> e

