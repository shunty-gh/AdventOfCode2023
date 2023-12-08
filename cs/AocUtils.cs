using Spectre.Console;

namespace Shunty.AoC;

public static class AocUtils
{
    public static async Task<IList<string>> GetDayLines(int day, string suffix = "")
    {
        var fn = FindInputFile(day, suffix);
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

    public static string FindInputFile(int day, string suffix = "")
    {
        var dstart = Directory.GetCurrentDirectory();
        var dir = dstart;
        var dayfile = string.IsNullOrWhiteSpace(suffix)
            ? $"day{day:D2}-input"
            : $"day{day:D2}-input-{suffix}";
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

public static class DictionaryExtensions
{
    /// <summary>
    /// A little IDictionary helper that checks if the key exists. If so it adds the value
    /// otherwise it sets the value. A bit like a Python defaultdict.
    /// </summary>
    public static void SetOrIncrement<T>(this IDictionary<T, int> dict, T key, int value)
    {
        if (dict.ContainsKey(key))
            dict[key] += value;
        else
            dict[key] = value;
    }
}

public static class IntExtensions
{
    public record Factor(Int64 Value, Int64 Power);

    public static IReadOnlyCollection<Factor> Factors(this Int64 value)
    {
        var factors = new Dictionary<Int64, Int64>();
        Int64 current = value;
        Int64 factor = 2;
        while (current > 1 && factor <= value / 2)
        {
            if (current % factor == 0)
            {
                if (factors.ContainsKey(factor))
                {
                    factors[factor] += 1;
                }
                else
                {
                    factors[factor] = 1;
                }
                current /= factor;
            }
            else
            {
                factor++;
            }
        }
        return factors.Select(kvp => new Factor(kvp.Key, kvp.Value)).OrderBy(x => x.Value).ToList();
    }

    /// <summary>
    /// Compute the least/lowest common multiple of two numbers.
    /// WARNING: This isn't actually tested. Use at someone else's risk.
    /// </summary>
    public static Int64 LCM(Int64 a, Int64 b)
    {
        var afacs = a.Factors();
        var bfacs = b.Factors();
        var allfacs = afacs.ToDictionary(a => a.Value, a => a.Power);
        foreach (var fac in bfacs)
        {
            if (allfacs.ContainsKey(fac.Value))
                allfacs[fac.Value] = Math.Max(allfacs[fac.Value], fac.Power);
            else
                allfacs[fac.Value] = fac.Power;
        }

        Int64 result = 1;
        foreach (var kvp in allfacs)
        {
            result *= (Int64)Math.Pow(kvp.Key, kvp.Value);
        }
        return result;
    }

    /// <summary>
    /// Compute the least/lowest common multiple of a list of numbers.
    /// WARNING: This has *never* been tested. Use at someone else's risk!
    /// It works for the uses I have put it to but there are no guarantees
    /// it will work in any other situation. Whatsoever.
    /// </summary>
    public static Int64 LCM(IEnumerable<Int64> nums)
    {
        if (nums.Count() == 0)
            return 0;
        if (nums.Count() == 1)
            return nums.First();

        var allfactors = nums.First().Factors().ToDictionary(a => a.Value, a => a.Power);
        foreach (var num in nums)
        {
            var nfacs = num.Factors();
            foreach (var fac in nfacs)
            {
                if (allfactors.ContainsKey(fac.Value))
                    allfactors[fac.Value] = Math.Max(allfactors[fac.Value], fac.Power);
                else
                    allfactors[fac.Value] = fac.Power;
            }
        }

        Int64 result = 1;
        foreach (var kvp in allfactors)
        {
            result *= (Int64)Math.Pow(kvp.Key, kvp.Value);
        }
        return result;
    }
}
