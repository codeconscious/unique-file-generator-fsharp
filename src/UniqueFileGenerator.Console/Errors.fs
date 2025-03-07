namespace UniqueFileGenerator.Console

open Utilities

module Errors =
    type ErrorType =
        | NoArgsPassed
        | ArgCountInvalid
        | FileCountInvalid of Arg: string * Floor: int * Ceiling: int
        | MalformedFlags
        | UnsupportedFlags
        | InvalidNumber of Arg: string * Floor: int * Ceiling: int
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | DriveSpaceInsufficient of Needed: string * Actual: string
        | UnknownError of string

    let getMessage error =
        match error with
        | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid argument count."
        | FileCountInvalid (x, f, c) ->
            $"Could not parse file count \"%s{x}\". Enter an integer between %s{formatInt f} and %s{formatInt c}, inclusive."
        | MalformedFlags -> "Malformed flag(s) found."
        | UnsupportedFlags -> "Unsupported flag(s) found."
        | InvalidNumber (x, f, c) ->
            $"Could not parse \"%s{x}\" to an integer between %s{formatInt f} and %s{formatInt c}, inclusive."
        | DirectoryMissing e -> $"Directory \"%s{e}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space. Though %s{needed} is necessary, only %s{actual} is available."
        | UnknownError e -> e

