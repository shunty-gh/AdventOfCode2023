using System.Collections.ObjectModel;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/12 - Hot Springs

/// <summary>
/// A very tidied up version of the first 1,000 failed attempts.
/// I just can't get recursion right without endless hours of head scratching and desk
/// thumping and computer cursing.
/// </summary>
///
public class Day12 : AocDaySolver
{
    public int DayNumber => 12;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var records = input.Select(l => l.Split(' '))
            .Select(s =>
                new ConditionRecord(
                    s[0],
                    s[1].Split(',')
                        .Select(int.Parse)
                        .ToArray()
                )
            )
            .ToList();

        // P1
        Int64 p1 = 0;
        foreach (var rec in records)
        {
            var perms = FindPerms(rec.Pattern, rec.Groups);
            p1 += perms;
        }
        this.ShowDayResult(1, p1);

        // P2
        Int64 p2 = 0;
        foreach (var rec in records)
        {
            // 'Multiply' each part by 5
            var groups = Enumerable.Repeat(rec.Groups, 5)
                .SelectMany(g => g)
                .ToArray();
            var pattern = string.Join('?', Enumerable.Repeat(rec.Pattern, 5));
            var perms = FindPerms(pattern, groups);
            p2 += perms;
        }
        this.ShowDayResult(2, p2);
    }

    private Dictionary<SearchState, Int64> PermCache = [];
    private Int64 FindPerms(string pattern, int[] groups)
    {
        // It's a bit slower to create a private cache variable here rather than use
        // the class cache 'PermCache'. But it is totally non-threadsafe should
        // we wish to go down that route.
        PermCache.Clear();
        return FindPermsRecursive(pattern.AsSpan(), groups.AsSpan(), new(0,0,0), PermCache);
    }

    private Int64 FindPermsRecursive(ReadOnlySpan<char> pattern, ReadOnlySpan<int> groups, SearchState state, IDictionary<SearchState, Int64> cache)
    {
        var (pi, gi, cgl) = state;
        // Have we been here before
        if (cache.ContainsKey(state))
            return cache[state];

        // Are we at the end of the pattern
        if (pi == pattern.Length)
        {
            // Have we matched the right number of groups
            // ie we've seen the correct number of groups AND we are not in the middle of processing another one
            // OR we've seen all but one group and we are currently processing a group and have read the same number of elements as the last group
            if ((gi == groups.Length && cgl == 0)
              || (gi == groups.Length - 1 && cgl == groups[gi]))
                return 1;
            else
                return 0;
        }

        // Otherwise process the next character
        Int64 result = 0;
        var c = pattern[pi];
        if (c == '.' || c == '?')
        {
            if (cgl == 0)
            {
                // We were not processing a group
                result += FindPermsRecursive(pattern, groups, new(pi+1, gi, 0), cache);
            }
            else if (gi < groups.Length && groups[gi] == cgl)
            {
                // We were processing a group AND we have seen the correct number of '#' characters
                // Reset the group length and update the number of found groups
                result += FindPermsRecursive(pattern, groups, new(pi+1, gi+1, 0), cache);
            }
            // else... we were processing a group and it turned out to have the wrong number
            // of '#' elements. Hence do no further processing on this particular path
        }
        if (c == '#' || c == '?')
        {
            // Start or continue processing a group - increment the current group length
            result += FindPermsRecursive(pattern, groups, new(pi+1, gi, cgl+1), cache);
        }

        cache[state] = result;
        return result;
    }
    private record ConditionRecord(string Pattern, int[] Groups);
    private record SearchState(int PatternIndex, int GroupIndex, int CurrentGroupLength);
}

