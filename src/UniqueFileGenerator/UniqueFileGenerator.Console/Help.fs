namespace UniqueFileGenerator.Console

open Printing
open ArgValidation

module Help =
    let private textBlocks = [
        [
            "Unique File Generator"
            "• Quickly and easily creates an arbitrary number of unique (by name and content) files"
            "• Accepts optional parameters to customize files according to your needs"
            "• Homepage: https://github.com/codeconscious/unique-file-generator-fsharp"
        ]
        [
            "Usage:"
        ]
        [
            "At the minimum, you must specify the number of files to generate. (Commas and underscores are ignored.)"
        ]
        [
            "If desired, files can be customized via the options below. You must supply a value for each option passed."
        ]
        [
            "-p"
            "    Prepends a specified prefix to each filename."
            "-b"
            "    The base filename length. Random alphanumeric characters will be used. The maximum is 100."
            $"    If not specified, defaults to %d{defaultOptions.NameBaseLength}."
            "-e"
            "    The extension to append to each generated filename. The initial period is optional."
            "    If not specified, no extension will be added."
            "-s"
            "    The size in bytes of each new file. Files will be populated with random alphanumeric characters."
            "    If not specified, each file will contain its own name."
            "-o"
            "    The output subdirectory in which files should be created. The directory must already exist."
            $"    If not specified, defaults to \"%s{defaultOptions.OutputDirectory}\"."
            "-d"
            "    A delay in milliseconds to be applied between each file's creation."
        ]
        [
            "Examples:"
        ]
        [
            "    dotnet run -- 10"
            "         Creates 10 files with the default settings"
        ]
        [
            "    dotnet run -- 1,000 -p TEST-1229 -b 10 -e txt -o \"My Output Folder\" -s 1000000 -d 1000"
            "         Creates one thousand 1MB files, each named like \"TEST-1229 ##########.txt\","
            "         in the existing subfolder \"My Output Folder\" with a 1s delay between new file."
        ]
        [
            "Note: `--` signals that the arguments are for this program and not the `dotnet` command."
        ]
    ]

    let print () =
        textBlocks
        |> List.iter (fun blockLines ->
            printEmptyLine ()
            blockLines |> List.iter printLine)
