using Spectre.Console;

namespace Shunty.AoC;

public static class AocUtils
{
    public static async Task<IEnumerable<string>> GetDayLines(int day)
    {
        var fn = FindInputFile(day);
        if (string.IsNullOrEmpty(fn))
            throw new FileNotFoundException($"Input file for day {day} not found");

        return (await File.ReadAllLinesAsync(fn))
            .Select(s => s.Trim([' ', '\r', '\n', '\t']))
            .ToList();
    }

    public static async Task<string> GetDayText(int day)
    {
        var fn = FindInputFile(day);
        if (string.IsNullOrEmpty(fn))
            throw new FileNotFoundException($"Input file for day {day} not found");

        return await File.ReadAllTextAsync(fn);
    }

    public static string FindInputFile(int day)
    {
        var dstart = Directory.GetCurrentDirectory();
        var dir = dstart;
        var dayfile = $"day{day:D2}-input";
        int maxParentLevels = 6;
        int parentLevel = 0;
        while (parentLevel <= maxParentLevels)
        {
            // Look in the directory
            var fn = Path.Combine(dir, dayfile);
            if (File.Exists(fn))
            {
                return fn;
            }

            // Look in ./input directory
            fn = Path.Combine(dir, "input", dayfile);
            if (File.Exists(fn))
            {
                return fn;
            }

            // Otherwise go up a directory
            var dinfo = Directory.GetParent(dir);
            if (dinfo == null)
            {
                break;
            }
            parentLevel++;
            dir = dinfo.FullName;
        }
        // Not found
        return "";
    }
}

public interface AocDaySolver
{
    int DayNumber { get; }
    Task Solve();
}

public static class AocExtensions
{
    private const string AllowSpectreWarnings = "The Spectre.Console warnings apply to methods we do not use";

    /// <summary>
    /// These extensions require the `Spectre.Console` Nuget package
    /// </summary>

    //[UnconditionalSuppressMessage("TrimAnalysis", "IL3002", Justification = AllowSpectreWarnings)]
    //[UnconditionalSuppressMessage("TrimAnalysis", "IL2104", Justification = AllowSpectreWarnings)]
    public static void ShowDayResult<T>(this AocDaySolver _, int part, T solution)
    {
        AnsiConsole.MarkupLine($"  [bold]Part {part}:[/] {solution?.ToString() ?? "<Unknown>"}");
    }

    public static void ShowDayResults<T>(this AocDaySolver _, T solution1, T solution2)
    {
        AnsiConsole.MarkupLine($"  [bold]Part 1:[/] {solution1?.ToString() ?? "<Unknown>"}");
        AnsiConsole.MarkupLine($"  [bold]Part 2:[/] {solution2?.ToString() ?? "<Unknown>"}");
    }

    public static void ShowDayResults<T1, T2>(this AocDaySolver _, T1 solution1, T2 solution2)
    {
        AnsiConsole.MarkupLine($"  [bold]Part 1:[/] {solution1?.ToString() ?? "<Unknown>"}");
        AnsiConsole.MarkupLine($"  [bold]Part 2:[/] {solution2?.ToString() ?? "<Unknown>"}");
    }
}
