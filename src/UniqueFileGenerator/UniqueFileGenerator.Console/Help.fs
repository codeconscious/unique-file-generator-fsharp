namespace UniqueFileGenerator.Console

module Help =
    open Spectre.Console

    let printInstructions () =
        let outerTable = Table (Width = 80, Border = TableBorder.HeavyHead)

        outerTable.AddColumn("Unique File Generator") |> ignore
        outerTable.AddRow("Quickly and easily create an arbitrary number of unique (by name and content) files.  " +
            "Each filename contains a random collection of characters.  " +
            "You can also supply optional parameters to customize files according to your needs.  " +
            "The tool checks that there is sufficient drive space available before starting.") |> ignore
        outerTable.AddEmptyRow() |> ignore

        outerTable.AddRow("At the minimum, you must specify the number of files to generate.  " +
            "This should be a sequence of numbers (with optional commas).") |> ignore
        outerTable.AddEmptyRow() |> ignore

        let argTable = Table()
        argTable.Border(TableBorder.None) |> ignore
        argTable.AddColumn("Flag") |> ignore
        argTable.AddColumn("Description") |> ignore
        argTable.HideHeaders() |> ignore
        argTable.Columns[0].PadRight(3) |> ignore
        argTable.AddRow("-p", "Add a filename prefix. If the prefix ends with a non-alphanumeric character, no space will be added after the prefix otherwise, one will be automatically added.") |> ignore
        argTable.AddRow("-e", "The file extension of the generated files. The opening period is optional. If not specified, no extension is added.") |> ignore
        argTable.AddRow("-s", "The desired size of each file in bytes, which will be populated with random characters. If not specified, each file will only contain its own name.") |> ignore
        argTable.AddRow("-o", "The output subfolder, which will be created if needed. If not supplied, \"output\" is used by default.") |> ignore
        argTable.AddRow("-d", "A delay in milliseconds to be applied between each file's creation. Defaults to 0 if unspecified.") |> ignore

        outerTable.AddRow(argTable) |> ignore
        outerTable.AddEmptyRow() |> ignore

        outerTable.AddRow("Examples:\n" +
            "   dotnet run 10\n" +
            "        Creates 10 files with the default settings\n" +
            "   dotnet run 1,000 -p TEST-1229 -e txt -o My Output Folder\n" +
            "                 -s 1000000 -d 1000\n" +
            "        Creates one thousand 1MB files, each named like\n" +
            "        \"TEST-1229 ##########.txt\", in a subfolder called\n" +
            "        \"My Output Folder\", with a 1s delay after each new file.") |> ignore

        outerTable.AddEmptyRow() |> ignore
        outerTable.AddRow("Homepage: https://github.com/codeconscious/unique-file-generator") |> ignore

        AnsiConsole.Write(outerTable)
        0
