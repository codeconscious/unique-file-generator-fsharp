namespace UniqueFileGenerator.Console

open Utilities

module Errors =

    type AppError =
        | NoArgsPassed
        | ArgCountInvalid
        | MalformedFlags
        | UnknownFlags
        | DuplicateFlags
        | ParseNumberFailure of Arg: string * AllowedRange: (int * int)
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | DriveSpaceInsufficient of Needed: string * Actual: string
        | IoError of string
        | CancelledByUser

    let errorMsg error =
        match error with
        | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid argument count."
        | MalformedFlags -> "Malformed flag(s) found."
        | UnknownFlags -> "Unknown flag(s) found."
        | DuplicateFlags -> "Duplicate option flag(s) found. Each can only be used once."
        | ParseNumberFailure (input, (floor, ceiling)) ->
            $"Cannot parse \"%s{input}\" to an integer between %s{Num.Format floor} and %s{Num.Format ceiling}, inclusive."
        | DirectoryMissing dirName -> $"Directory \"%s{dirName}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space. Though %s{needed} is necessary, only %s{actual} is available."
        | IoError msg -> $"IO error: %s{msg}"
        | CancelledByUser -> "Cancelled."

