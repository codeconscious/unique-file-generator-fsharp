# Unique File Generator (F# Version)

- Quickly and easily creates an arbitrary number of unique (by name and content) files
- Accepts optional parameters to customize files according to your needs
- Requirement: .NET 9 (or higher) runtime

(I also have very similar tools written in [C#](https://github.com/codeconscious/unique-file-generator-csharp) and [Rust](https://github.com/codeconscious/unique-file-generator-rust/) since this has apparently become one of my go-to projects for language practice. 😅)


## Usage

At the minimum, you must specify the number of files you want to generate. This should be a single positive integer (with optional commas or underscores).

```
dotnet run -- 1000
```

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

> [!NOTE]
> `--` is needed after `dotnet run` to signal that the arguments are for this application and not the `dotnet` command.

```
dotnet run -- 50,000 -p Random-
```

Creates 50,000 files, each named similarly to "Random-##########...", in a subfolder named "output". There are no extensions.

```
dotnet run -- 100 -p "TEST " -b 10 -e txt -o "My Output Folder" -s 1000000 -d 1000
```

Creates 100 1MB files, each named similarly to "TEST ##########.txt", with a 1-second break between each file's creation, in a subfolder called "My Output Folder".
