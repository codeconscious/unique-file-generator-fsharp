namespace UniqueFileGenerator.Console

module Io =
    open System.IO
    open ArgValidation.Types

    let createFile directory fileName (contents: string) =
        try
            let path = Path.Combine(directory, fileName)
            File.WriteAllText(path, contents)
            Ok fileName
        with
            | e -> Error $"%s{e.Message}"

    let verifyDirectory dir =
        Directory.Exists dir

