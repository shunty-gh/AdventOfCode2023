using System.Text.RegularExpressions;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/9 - Mirage Maintenance

public class Day09 : AocDaySolver
{
    public int DayNumber => 9;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var re = new Regex(@"[-]*\d+");
        var nums = new List<int[]>();
        foreach (var line in input)
        {
            var arr = re.Matches(line)
                .Select(m => int.Parse(m.Value))
                .ToArray();
            nums.Add(arr);
        }

        var predictions = new List<(int P1, int P2)>();
        foreach (var n in nums)
        {
            predictions.Add(ReduceNums(n));
        }
        this.ShowDayResult(1, predictions.Sum(e => e.P1));
        this.ShowDayResult(2, predictions.Sum(e => e.P2));
    }

    private (int P1, int P2) ReduceNums(int[] nums)
    {
        var firsts = new List<int>(); // Placeholder for first element at each stage
        var lasts = new List<int>(); // Placeholder for last element at each stage
        var current = nums;
        while (current.Length > 1)
        {
            lasts.Add(current.Last());
            firsts.Add(current.First());

            var next = new int[current.Length - 1];
            for (var i = 0; i < current.Length - 1; i++)
            {
                next[i] = current[i+1] - current[i];
            }
            current = next;
        }

        var nx = 0;
        for (var i = firsts.Count - 1; i >= 0; i--)
        {
            nx = firsts[i] - nx;
        }
        return (lasts.Sum(), nx);
    }
}
