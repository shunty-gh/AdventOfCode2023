namespace Shunty.AoC;

// https://adventofcode.com/2023/day/5 - If You Give A Seed A Fertilizer

/// A (much) faster version than my original brute force approach. What
/// are the chances of being able to understand it in weeks/years to come?

public class Day05 : AocDaySolver
{
    public int DayNumber => 5;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);

        var seeds = input[0].Split(':')[1].Trim().Split(' ').Select(Int64.Parse).ToList();
        var maps = new List<List<MapRule>>();
        var currentRules = new List<MapRule>();
        foreach (var line in input.Skip(2))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line.Trim().EndsWith("map:"))
            {
                if (currentRules.Count > 0)
                    maps.Add(currentRules);
                currentRules = [];
            }
            else
            {
                var mapsplit = line.Split(' ').Select(Int64.Parse).ToArray();
                currentRules.Add(new(mapsplit[0], mapsplit[1], mapsplit[2]));
            }
        }
        // Don't forget to add the last set
        maps.Add(currentRules);

        // Part 1
        Int64 p1 = -1;
        foreach (var seed in seeds)
        {
            var s = seed;
            foreach (var map in maps)
            {
                s = ProcessSeed(s, map);
            }
            if (p1 < 0 || p1 > s)
                p1 = s;
        }
        this.ShowDayResult(1, p1);


        // Part 2
        int seedid = 0;
        var p2ranges = new List<SeedRange>();
        while (seedid < seeds.Count)
        {
            p2ranges.Add(new(seeds[seedid], seeds[seedid + 1]));
            seedid += 2;
        }

        foreach (var map in maps)
        {
            var next = ProcessRanges(p2ranges, map);
            p2ranges.Clear();
            p2ranges.AddRange(next.Distinct());
        }
        // The final collection of p2ranges will be the set of locations. Get the smallest.
        var p2 = p2ranges.Min(s => s.Start);
        this.ShowDayResult(2, p2);
    }


    private IList<SeedRange> ProcessRanges(IEnumerable<SeedRange> ranges, IEnumerable<MapRule> ruleGroup)
    {
        var result = new List<SeedRange>();
        foreach (var rng in ranges)
        {
            var rsplit = ProcessRange(rng, ruleGroup);
            result.AddRange(rsplit);
        }
        return result;
    }

    /// <summary>
    /// Apply each rule in the rule group to a single seed range
    /// </summary>
    /// <returns>A collection of SeedRanges that have had the entire rule group applied to them.
    /// </returns>
    private IList<SeedRange> ProcessRange(SeedRange range, IEnumerable<MapRule> ruleGroup)
    {
        var result = new List<SeedRange>();
        var to_test = new List<SeedRange> { range };
        var remmain = new List<SeedRange>();
        foreach (var rule in ruleGroup)
        {
            remmain.Clear();
            foreach (var rng in to_test)
            {
                var (unch, rule_applied) = ApplyRule(rng, rule);
                if (rule_applied is not null)
                    result.Add(rule_applied);
                remmain.AddRange(unch);
            }
            to_test.Clear();
            to_test.AddRange(remmain);
        }
        // Add any ranges that are still unchanged to the result
        result.AddRange(to_test);
        return result;
    }

    /// <summary>
    /// A single rule, when applied to a single seed range, can yield up to three new ranges:
    /// <para/>1) seeds that have a lower number than the start of the range - will remain unchanged
    /// <para/>2) seeds that fall within then bounds of the rule - will produce a single new seed range of changed items
    /// <para/>3) seeds that have a higher number than the top end of the rule range - will remain unchanged
    /// <para>
    /// Therefore rather than having to test the rule against every single item in the range we will
    /// apply the rule over chunks of the given range and return the new ranges.
    /// </para>
    /// </summary>
    /// <param name="range">The single seed range to test</param>
    /// <param name="rule">The single rule to test against</param>
    /// <returns>A tuple containing a list of 0, 1 or 2 unchanged SeedRanges and a SeedRange of updated
    /// seeds or null if none match the rule</returns>
    private (IList<SeedRange> unchanged, SeedRange? changed) ApplyRule(SeedRange range, MapRule rule)
    {
        if ((range.Start > rule.SourceEnd) || (range.End < rule.SourceStart)) // No overlap, return source range as-is
        {
           return (new List<SeedRange> { range }, null);
        }
        else if ((range.Start >= rule.SourceStart) && (range.End <= rule.SourceEnd)) // totally enclosed, apply rule to whole range
        {
            var offset = range.Start - rule.SourceStart;
            var rstart = rule.DestStart + offset;
            return ([], new(rstart, range.RangeLength));
        }
        else // must be some overlap
        {
            var unchanged = new List<SeedRange>();
            // Before/less-than the rule - unchanged values
            if (range.Start < rule.SourceStart)
            {
                unchanged.Add(new(range.Start, rule.SourceStart - range.Start));
            }
            // Within the rule - apply rule
            var srcstart = Math.Max(range.Start, rule.SourceStart);
            var offset = srcstart - rule.SourceStart;
            var dststart = rule.DestStart + offset;
            var srcend = Math.Min(range.End, rule.SourceEnd);
            var changed = new SeedRange(dststart, srcend - srcstart + 1);
            // After/greater-than the rule - unchanged values
            if (range.End > rule.SourceEnd)
            {
                unchanged.Add(new(rule.SourceEnd + 1, range.End - rule.SourceEnd));
            }
            return (unchanged, changed);
        }
    }

    private Int64 ProcessSeed(Int64 source, List<MapRule> rules)
    {
        foreach (var rule in rules)
        {
            if (source >= rule.SourceStart && source <= rule.SourceEnd)
            {
                return rule.DestStart + (source - rule.SourceStart);
            }
        }
        // if not found, return same as source
        return source;
    }

}

public record MapRule(Int64 DestStart, Int64 SourceStart, Int64 RangeLength)
{
    public Int64 SourceEnd => SourceStart + RangeLength - 1;
}

public record SeedRange(Int64 Start, Int64 RangeLength)
{
    public Int64 End => Start + RangeLength - 1;
}
