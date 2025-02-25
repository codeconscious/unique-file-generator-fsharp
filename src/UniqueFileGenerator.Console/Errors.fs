namespace UniqueFileGenerator.Console

module Errors =

    type StartupError =
        | NoArgsPassed
        | ArgCountInvalid
        | FileCountInvalid of string
        | MalformedFlags
        | UnsupportedFlags
        | DirectoryMissing of string
