namespace UniqueFileGenerator.Console

open Printing

module Help =
    let print () =
        printEmptyLine ()
        printLine "Unique File Generator"
        printLine "• Quickly and easily create an arbitrary number of unique (by name and content) files."
        printLine "• Each filename contains a random collection of characters."
        printLine "• You can also supply optional parameters to customize files according to your needs."
        printLine "• The tool checks that there is sufficient drive space available before starting."
        printEmptyLine ()
        printLine "At the minimum, you must specify the number of files to generate."
        printEmptyLine ()
        printLine "Optional flags:"
        printEmptyLine ()
        printLine "-p"
        printLine "    Add a filename prefix."
        printLine "-e"
        printLine "    The file extension of the generated files. The initial period is optional."
        printLine "-s"
        printLine "    The desired size of each file in bytes. Files will be populated with random characters."
        printLine "    If not specified, each file will only contain its own name."
        printLine "-o"
        printLine "    The output subfolder, which must exist."
        printLine "    If not specified, \"output\" is used by default."
        printLine "-d"
        printLine "    A delay in milliseconds to be applied between each file's creation."
        printEmptyLine ()
        printLine "Examples:"
        printEmptyLine ()
        printLine "    dotnet run 10"
        printLine "         Creates 10 files with the default settings"
        printEmptyLine ()
        printLine "    dotnet run 1,000 -p TEST-1229 -e txt -o My Output Folder"
        printLine "                  -s 1000000 -d 1000"
        printLine "         Creates one thousand 1MB files, each named like"
        printLine "         \"TEST-1229 ##########.txt\", in a subfolder called"
        printLine "         \"My Output Folder\", with a 1s delay after each new file."
        printEmptyLine ()
        printLine "Homepage: https://github.com/codeconscious/"

