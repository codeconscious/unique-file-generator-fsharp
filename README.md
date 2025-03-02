# Unique File Generator (F# Version)

- Quickly and easily create an arbitrary number of unique (by name and content) files
- Use optional parameters to customize files according to your needs
- Checks remaining drive space to ensure users do not accidentally fill their drive
- Requirement: .NET 9 (or higher) runtime

## But... why?

In a previous position, I had use for such a utility once to avoid unneeded warnings when testing file uploads in a system that warned about duplicate files and filenames.

However, I only created _this_ utility to get more experience with F# and functional programming in general. (I have also written similar tools in [C#](https://github.com/codeconscious/unique-file-generator-csharp) and [Rust](https://github.com/codeconscious/unique-file-generator-rust/) before, so it seems this has become one of my go-to projects for getting experience with languages. 😅)

## Usage

At the minimum, you must specify the number of files you want to generate with the default options. This should be a single positive integer (with optional commas or underscores).

```sh
dotnet run -- 1000
```

> [!NOTE]
> `--` is needed after `dotnet run` to signal that the arguments are for this application and not the `dotnet` command.

### Options

If desired, files can be customized via the options below. You must supply a value for each option passed.

Flag | Description
---- | :----
-p | Prepends a specified prefix to each filename.
-b | The base filename length. Random alphanumeric characters will be used.  The maximum is 100. If not specified, a default of 50 will be used.
-e | The extension to append to each generated filename. The initial period is optional. If not specified, no extension will be added.
-s | The size in bytes of each new file. Files will be populated with random alphanumeric characters. If not specified, each file will contain its own name.
-o | The output subdirectory in which files should be created. The directory must already exist. If not specified, defaults to "output".
-d | A delay in milliseconds to be applied between each file's creation.

### Examples

```sh
dotnet run -- 50,000 -p Random-
```

Creates 50,000 files, each named similarly to "Random-##########", in a subfolder named "output". No files have extensions.

```sh
dotnet run -- 100 -p "TEST " -b 7 -e txt -o "My Output Folder" -s 1000000 -d 1000
```

Creates 100 1MB files, each named similarly to "TEST #######.txt", with a 1-second break between each file's creation, in a subfolder called "My Output Folder".

```sh
dotnet run -- --help
```

See the in-program instructions.
