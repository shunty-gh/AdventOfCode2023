namespace Shunty.AoC;

// https://adventofcode.com/2023/day/1 - Trebuchet?!

public class Day01 : AocDaySolver
{
    public static int DayNumber() => 1;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber());

        // Part 1
        var sum = 0;
        foreach (var s in input)
        {
            var n = s.Where(c => c >= '0' && c <= '9');
            sum += ((int)(n.First() - '0') * 10) + (int)(n.Last() - '0');
        }
        this.ShowDayResult(1, sum);

        // Part 2
        var digitmap = new Dictionary<string, int>
        {
            {"0", 0},{"1", 1},{"2", 2},{"3", 3},{"4", 4},{"5", 5},{"6", 6},{"7", 7},{"8", 8},{"9", 9},
            {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4}, {"five", 5},
            {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9},
        };
        var sum2 = 0;
        foreach (var s in input)
        {
            var first = -1;
            var last = 0;
            for (var index = 0; index < s.Length; index++)
            {
                foreach (var (k,v) in digitmap)
                {
                    if (s.AsSpan(index).StartsWith(k))
                    {
                        if (first == -1)
                            first = v * 10;
                        last = v;
                        // Don't do this! What about "...nineight..." or "...threeightwo.." types of sequences
                        //index += k.Length - 1;
                        if (k.Length > 1)
                            index += k.Length - 2;
                        break;
                    }
                }
            }
            sum2 += first + last;
        }
        this.ShowDayResult(2, sum2);
    }
}