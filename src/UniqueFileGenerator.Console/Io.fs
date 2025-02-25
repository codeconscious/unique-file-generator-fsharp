namespace UniqueFileGenerator.Console

module Io =
    open System.IO
    open Errors

    let verifyDirectory dir =
        match Directory.Exists dir with
        | true -> Ok dir
        | false -> Error (DirectoryMissing dir)

    let createFile directory fileName (contents: string) =
        try
            let path = Path.Combine(directory, fileName)
            File.WriteAllText(path, contents)
            Ok fileName
        with
            | e -> Error $"%s{e.Message}"
