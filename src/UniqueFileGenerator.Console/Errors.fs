namespace UniqueFileGenerator.Console

open Utilities

module Errors =

    type AppError =
        | ArgsMissing
        | ArgCountInvalid
        | MalformedFlags
        | UnknownFlags
        | DuplicateFlags
        | NumberParseFailure of Input: string * AllowedRange: (int * int)
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | DriveSpaceInsufficient of Needed: string * Actual: string
        | IoError of string
        | CancelledByUser

    let errorMsg = function
        | ArgsMissing -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid arguments. If you submit option flags, each must have a corresponding value."
        | MalformedFlags -> "Malformed option flag(s) found."
        | UnknownFlags -> "Unknown option flag(s) found."
        | DuplicateFlags -> "Duplicate option flag(s) found. Each can only be used once."
        | NumberParseFailure (input, (floor, ceiling)) ->
            $"The number \"%s{input}\" is out of bounds. Enter an integer between %s{Num.Format floor} and %s{Num.Format ceiling}, inclusive."
        | DirectoryMissing dirName -> $"Directory \"%s{dirName}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | DriveSpaceInsufficient (needed, actual) ->
            $"Insufficient drive space: %s{needed} is necessary, but only %s{actual} is available."
        | IoError msg -> $"IO error: %s{msg}"
        | CancelledByUser -> "Cancelled by the user."

