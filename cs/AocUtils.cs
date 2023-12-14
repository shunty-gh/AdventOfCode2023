using System.Numerics;
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

public static class NumericExtensions
{
    public record Factor(Int64 Value, Int64 Power);

    public static Int64 Sum64(this IEnumerable<int> source)
    {
        Int64 result = 0;
        foreach (int n in source)
        {
            result += n;
        }
        return result;
    }

    public static Int64 Sum64(this IEnumerable<Int64> source)
    {
        Int64 result = 0;
        foreach (int n in source)
        {
            result += n;
        }
        return result;
    }

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
    [Obsolete("Rewritten as LeastCommonMultiple")]
    public static Int64 LCMObsolete(Int64 a, Int64 b)
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
    [Obsolete("Rewritten as LeastCommonMultiple")]
    public static Int64 LCMObsolete(IEnumerable<Int64> nums)
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

    public static T GCD<T>(T a, T b) where T : INumber<T> => GreatestCommonFactor(a, b);
    public static T GCF<T>(T a, T b) where T : INumber<T> => GreatestCommonFactor(a, b);
    public static T GreatestCommonFactor<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.Zero)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    public static T LeastCommonMultiple<T>(T a, T b) where T : INumber<T>
    {
        checked
        {
            return a  / GreatestCommonFactor(a, b) * b;
        }
    }

    public static T LCM<T>(this IEnumerable<T> values) where T : INumber<T> => LeastCommonMultiple(values);
    public static T LeastCommonMultiple<T>(this IEnumerable<T> values) where T : INumber<T>
    {
        checked
        {
            return values.Aggregate(LeastCommonMultiple);
        }
    }

    public static (int StartIndex, int SequenceLength) FindRepeatingSequence<T>(this List<T> source, int minRequiredSeqLength = 2) where T: INumber<T>
    {
        for (var i = 0; i < source.Count; i++)
        {
            for (var j = i + minRequiredSeqLength; j < source.Count; j++)
            {
                // Find a matching load value
                if (source[i] == source[j])
                {
                    // Is the sequence to get there the same?
                    var rlen = j - i;
                    if (j + rlen >= source.Count)
                        break;
                    if (source[i..j].SequenceEqual(source[j..(j + rlen)]))
                    {
                        // If we can, then make sure the next sequence also matches just for good measure
                        if (j+rlen+rlen >= source.Count || source[i..j].SequenceEqual(source[(j+rlen)..(j+rlen+rlen)]))
                        {
                            //Console.WriteLine($"Found range starting at i={i} for {j - i} elements");
                            return (i, j - i);
                        }
                    }
                }
            }
        }
        return (-1, -1);
    }

}