# Advent Of Code 2023

## C# solutions

Some solutions written in C# / .Net 8 for the [Advent of Code 2023](https://adventofcode.com/2023) puzzles. All nice and polite and cross-platform.

### Pre-requisites

* [.Net 8 installed](https://dotnet.microsoft.com/download/dotnet)
* (Optional) [Visual Studio Code](https://code.visualstudio.com/) - the build and run/debug tasks work with VS Code
* (Optional) Any other C# code editor, notepad, IDE, environment, cloud, whatever you so desire
* (Optional) If using VS Code it would be a good idea to install the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension

### Run

```
cd <path-to-this-cs-directory>
dotnet run
```

or add `--project ./cs` to the line if running from the repository root directory.


By default, with no command line arguments, it will run the solution for the current day if we are within 1-25 December 2023 range or it will run all solutions if we are not within the puzzle date range. To specify one or more specific days just list them on the command line separated by spaces. Obviously it will only run days where the solution has been written. eg:

```
dotnet run 1 11 19
```

To run all available solutions use the `--all` flag on the command line. ie:

```
dotnet run --all
```

### Debugging

To debug in Visual Studio Code set a breakpoint by clicking in the gutter on the source code line of your choice then press F5 (you might need to select the appropriate debug profile).
