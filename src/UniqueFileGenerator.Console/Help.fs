namespace UniqueFileGenerator.Console

open System
open ArgTypes
open Printing

[<RequireQualifiedAccess>]
module Help =
    let private helpFlag = "--help"

    let wasRequested (args: string array) =
        args.Length > 0 &&
        args[0].Trim().Equals(helpFlag, StringComparison.InvariantCultureIgnoreCase)

    let private textBlocks = [
        [
            "Unique File Generator"
            "• Quickly and easily creates an arbitrary number of unique (by name and content) files"
            "• Accepts optional parameters to customize files according to your needs"
            "• Checks remaining drive space to ensure the drive is not unintentionally filled"
            "• Homepage: https://github.com/codeconscious/unique-file-generator-fsharp"
        ]
        [
            "At the minimum, you must specify the number of files to generate with the default options."
            "(Commas and underscores are ignored.)"
        ]
        [
            "If desired, files can be customized via the options below. You must supply a value for each option passed."
        ]
        [
            "-p"
            "    Prepends a specified prefix to each filename."
            "    If not specified, no prefix will be added."
            "-b"
            "    The base filename length. Random alphanumeric characters will be used. The maximum is 100."
            $"    If not specified, defaults to %d{NameBaseLength.DefaultValue}."
            "-e"
            "    The extension to append to each generated filename. The initial period is optional."
            "    If not specified, no extension will be added."
            "-s"
            "    The size in bytes of each new file. Files will be populated with random alphanumeric characters."
            "    If not specified, each file will contain its own name."
            "-o"
            "    The output subdirectory in which files should be created. The directory must already exist."
            $"    If not specified, defaults to \"%s{OutputDirectory.DefaultValue}\"."
            "-d"
            "    A delay in milliseconds to be applied between each file's creation."
            "    If not specified, no delay is applied."
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
        |> List.iteri (fun i blockLines ->
            if i = 0 then () else printEmptyLine ()
            blockLines |> List.iter printLine)

    let suggest () =
        printfn "Pass \"--help\" to see the program instructions."
