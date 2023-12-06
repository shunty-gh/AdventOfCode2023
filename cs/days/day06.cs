using System.Text.RegularExpressions;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/6 - Wait For It

public class Day06 : AocDaySolver
{
    public int DayNumber => 6;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);
        var re = new Regex(@"\d+");
	    var nums = re.Matches(input[0])
            .Select(m => int.Parse(m.Value))
            .Zip(re.Matches(input[1])
                .Select(m => int.Parse(m.Value)));

        var allways = new List<int>();
        foreach (var (tm, ds) in nums)
        {
            int ways = 0, t1 = 1;
            while (t1 <= tm / 2)
            {
                if (t1 * (tm - t1) > ds)
                    ways += 1;
                t1++;
            }
            var w = tm % 2 == 0 ? (ways * 2) - 1 : ways * 2;
            allways.Add(w);
        }
        var p1 = allways.Aggregate(1, (acc,v) => acc * v);
        this.ShowDayResult(1, p1);


    	var nums2 = nums.Aggregate(("",""), (acc, v) => (acc.Item1 + v.First.ToString(), acc.Item2 + v.Second.ToString()));
        Int64 tm2 = Int64.Parse(nums2.Item1);
        Int64 ds2 = Int64.Parse(nums2.Item2);
        Int64 ways2 = 0, t2 = 1;
        while (t2 <= tm2 / 2)
        {
            if (t2 * (tm2 - t2) > ds2)
                ways2 += 1;
            t2++;
        }

        Int64 p2 = tm2 % 2 == 0 ? (ways2 * 2) - 1 : ways2 * 2;
        this.ShowDayResult(2, p2);
    }
}
