namespace UniqueFileGenerator.Console

module Errors =
    type StartupError =
        | NoArgsPassed
        | ArgCountInvalid
        | FileCountInvalid of string
        | MalformedFlags
        | UnsupportedFlags
        | DirectoryMissing of string
        | DriveSpaceConfirmationFailure
        | InsufficientDriveSpace of Needed: int64 * Actual: int64

    let getMessage error =
        match error with
        | NoArgsPassed -> "You must pass in at least one argument: the number of files to generate."
        | ArgCountInvalid -> "Invalid argument count."
        | FileCountInvalid c -> $"Invalid file count: %s{c}."
        | MalformedFlags -> "Malformed flag(s) found."
        | UnsupportedFlags -> "Unsupported flag(s) found."
        | DirectoryMissing e -> $"Directory \"%s{e}\" was not found."
        | DriveSpaceConfirmationFailure -> "Could not confirm available drive space."
        | InsufficientDriveSpace (needed, actual) ->
            sprintf "Not enough drive space available. %d bytes are necessary, but only %d are available."
                    needed actual

