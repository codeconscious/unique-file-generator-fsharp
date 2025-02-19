# Unique File Generator (F# Version)

This command line tool allows you to quickly and easily create an arbitrary number of unique (by name and content) files on your computer. Each filename contains a random collection of characters to differentiate them. You can also supply optional parameters to customize files according to your needs.

※ I also have very similar tools written in C# and [Rust](https://github.com/codeconscious/unique-file-generator-rust/blob/main/README.md) since this has apparently become one of my go-to projects to practice languages. 😅)

## Requirements

- .NET 9 (or higher) runtime

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
-b | The base filename length. Random alphanumeric characters will be used. If not specified, a default of 50 will be used.
-e | The extension to append to each generated filename. The initial period is optional. If not specified, no extension will be added.
-s | The size in bytes of each new file. Files will be populated with random alphanumeric characters. If not specified, each file will contain its own name.
-o | The output subdirectory in which files should be created. The directory must already exist. If not specified, defaults to "output".
-d | A delay in milliseconds to be applied between each file's creation.

### Examples

Note: `--` is needed after `dotnet run` to signal that the parameters are for this application and not for the `dotnet` command.

```
dotnet run -- 50,000 -p Random-
```
Creates 50,000 files, each named similarly to "Random-########", in a subfolder named "output". There are no prefixes or extensions.

```
dotnet run -- 100 -p TEST-1229 -e txt -o My Output Folder -s 1000000 -d 1000
```
Creates one hundred 1MB files, each named similarly to "TEST-1229 ##########.txt", with a 1s break between each file's creation, and in a subfolder called "My Output Folder".
