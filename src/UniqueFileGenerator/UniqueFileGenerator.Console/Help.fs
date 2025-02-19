namespace UniqueFileGenerator.Console

open Printing

module Help =
    let print () =
        printEmptyLine ()
        printLine "Unique File Generator"
        printLine "• Quickly and easily creates an arbitrary number of unique (by name and content) files."
        printLine "• Accepts optional parameters to customize files according to your needs."
        printLine "• Requirement: .NET 9 (or higher) runtime"
        printLine "• Homepage: https://github.com/codeconscious/unique-file-generator-fsharp"
        printEmptyLine ()
        printLine "Usage:"
        printEmptyLine ()
        printLine "At the minimum, you must specify the number of files to generate as a single, positive integer."
        printLine "(Commas and underscores will be ignored.)"
        printEmptyLine ()
        printLine "If desired, files can be customized via the options below. You must supply a value for each option passed."
        printEmptyLine ()
        printLine "-p"
        printLine "    Prepends a specified prefix to each filename."
        printLine "-b"
        printLine "    The base filename length. Random alphanumeric characters will be used."
        printLine $"    If not specified, defaults to {ArgValidation.defaultNameBaseLength}."
        printLine "-e"
        printLine "    The extension to append to each generated filename. The initial period is optional."
        printLine "    If not specified, no extension will be added."
        printLine "-s"
        printLine "    The size in bytes of each new file. Files will be populated with random alphanumeric characters."
        printLine "    If not specified, each file will contain its own name."
        printLine "-o"
        printLine "    The output subdirectory in which files should be created. The directory must already exist."
        printLine $"    If not specified, defaults to \"{ArgValidation.defaultOutputDirectory}\"."
        printLine "-d"
        printLine "    A delay in milliseconds to be applied between each file's creation."
        printEmptyLine ()
        printLine "Examples:"
        printEmptyLine ()
        printLine "    dotnet run -- 10"
        printLine "         Creates 10 files with the default settings"
        printEmptyLine ()
        printLine "    dotnet run -- 1,000 -p TEST-1229 -b 10 -e txt -o \"My Output Folder\" -s 1000000 -d 1000"
        printLine "         Creates one thousand 1MB files, each named like"
        printLine "         \"TEST-1229 ##########.txt\", in the existing subfolder"
        printLine "         \"My Output Folder\", with a 1s delay for each new file."
        printEmptyLine ()
        printLine "Note: `--` signals that the arguments are for this program and not the `dotnet` command."
        printEmptyLine ()
