namespace Shunty.AoC;

// https://adventofcode.com/2023/day/8

public class Day08 : AocDaySolver
{
    public int DayNumber => 8;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);
        var instructions = input[0].ToCharArray().Select(c => c == 'L' ? 0 : 1).ToList();
        var elements = new Dictionary<string, string[]>();
        foreach (var line in input.Skip(2))
        {
            var sp = line.Split(" = ");
            elements.Add(sp[0].Trim(), sp[1].Trim(['(',')']).Split(", "));
        }
        // P1
        var el = "AAA";
        var steps = 0;
        var index = 0;
        var ilen = instructions.Count;
        while (el != "ZZZ")
        {
            steps++;
            el = elements[el][instructions[index]];
            index = (index+1) % ilen;
        }
        this.ShowDayResult(1, steps);

        // P2
        // Brief analysis of the input shows that there are only 5 elements that end in 'A' (in this particular case).
        // If we run the process for each one we find that they all 'terminate' at different
        // points but that each 'process' repeats over a constant number of steps/cycles.
        // So all we need to do is to find the loop period for each start element and then find
        // the lowest common multiple of all of the periods.
        // Start by running the process for a large-ish number of cycles to ensure we find the loop
        // count for each element. It turns out they all loop at around 20k (for our input).
        var els = elements.Where(kvp => kvp.Key.EndsWith('A')).Select(kvp => kvp.Key).ToArray();
        var elen = els.Length;
        var laststeps = new int[els.Length];
        var loopPeriod = new int[els.Length];
        // Pick an arbitrary max no of loops - but big enough to be sure to cover all
        // completions for each input
        var maxLoops = 50_000;
        steps = 0;
        index = 0;
        while (steps < maxLoops)
        {
            steps++;
            for (var i = 0; i < elen; i++)
            {
                var current = els[i];
                var next = elements[current][instructions[index]];
                els[i] = next;
                if (next.EndsWith('Z'))
                {
                    var period = steps - laststeps[i];
                    loopPeriod[i] = period;
                    laststeps[i] = steps;
                    // Use at the start to get an idea of the looping
                    //Console.WriteLine($"{steps} | {period} | {el3}");
                }
            }
            index = (index+1) % ilen;
        }

        var p2 = loopPeriod.Select(Convert.ToInt64).LeastCommonMultiple();
        this.ShowDayResult(2, p2);
    }
}
