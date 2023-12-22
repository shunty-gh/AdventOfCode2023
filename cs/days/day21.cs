namespace Shunty.AoC;

// https://adventofcode.com/2023/day/21 - Step Counter

/* This is one I never want to see again.
   I admit, I had to get help from Reddit for part 2. Turns out that because of
   various things like the empty row and col across the middle of the input and
   the empty rows and cols around the borders of the input then the input somehow
   makes it into a solvable quadratic equation.
   I'd managed to work out that once a map block gets filled then it flips
   between an odd and an even state of accessible points. Also that each other
   map copy does the same after a period equal to the map length. This is related
   to the "blank row across the middle" thingy.
   Once you know that you have to look for the polynolmial coefficients and then
   have to plug them in to the standard quadratic equation it's quite straightforward.
   But how in gods name I'd have found that out for myself I have no idea.
 */

public class Day21 : AocDaySolver
{
    public int DayNumber => 21;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var sy = input.FindIndex(ln => ln.Contains('S'));
        var sx = input[sy].IndexOf('S');

        this.ShowDayResult(1, Part1(input, sx, sy, 64));
        this.ShowDayResult(2, Part2(input, sx, sy, 26501365));
    }

    private long Part2(List<string> input, int sx, int sy, int totalStepsTarget)
    {
        //var debuglines = new List<string>();
        //var stepCounts = new List<long>();
        int xr = input[0].Length, yr = input.Count;
        var (dv,rm) = Math.DivRem(totalStepsTarget, xr);
        // Look for the result of three sets of steps to give f(0), f(1), f(2)
        // so we can calculate the coefficients of a quadratic.
        // First is the offset/remainder then the result for further full
        // rounds of the map ie +(map length) and +(2 x map length)
        int[] targetSteps = [rm, rm + xr, rm + xr + xr];
        int[] fx = [0,0,0];
        int fxindex = 0;

        var seen = new HashSet<(long,long)> { (sx,sy) };
        for (var steps = 0; steps < targetSteps[2]; steps++)
        {
            var tovisit = seen.ToList();
            seen.Clear();
            foreach (var (cx,cy) in tovisit)
            {
                var neigh = Neighbours(input, cx, cy, 2);
                foreach (var (nx,ny) in neigh)
                {
                    if (seen.Contains((nx,ny)))
                        continue;
                    seen.Add((nx,ny));
                }
            }
            if (targetSteps.Contains(steps+1))
            {
                fx[fxindex] = seen.Count;
                fxindex++;
            }

            // Debug - calculate counts of seen positions for a grid of (2xlim)+1
            // repeated squares. Then save them to file.
            //
            // stepCounts.Add(seen.Count);
            // var lim = 5;
            // var qcount = new Dictionary<(int,int), int>();
            // for (var my = -lim; my <= lim; my++)
            // {
            //     for (var mx = -lim; mx <= lim; mx++)
            //     {
            //         int xl = mx * xr, yl = my * yr, xh = xl+xr, yh = yl+yr;
            //         qcount[(xl,yl)] = 0;
            //         foreach (var (x,y) in seen)
            //         {
            //             if (x >= xl && x < xh && y >= yl && y < yh)
            //             {
            //                 qcount[(xl,yl)] += 1;
            //             }
            //         }
            //     }
            // }
            //
            // var ln = $"{steps+1:D5}, {stepCounts.Sum64():D8}, ";
            // ln += string.Join(", ", qcount.OrderBy(kvp => kvp.Key.Item2)
            //     .ThenBy(kvp => kvp.Key.Item1)
            //     .Select(i => $"{i.Value:D4}"));
            // debuglines.Add(ln);

            //Console.WriteLine(ln);
        }
        // var fdir = Path.GetDirectoryName(AocUtils.FindInputFile(DayNumber)) ?? "";
        // var fn = Path.Combine(fdir, $"day{DayNumber:D2}-debug-lines");
        // File.WriteAllLines(fn, debuglines);
        // Console.WriteLine($"For quadratic equation use x = {dv}");
        // Console.WriteLine("Paste the following line into the WolframAlpha quadratic fit calculator at https://www.wolframalpha.com/input?i=quadratic+fit");
        // Console.WriteLine($"{{ {{0,{fx[0]}}}, {{1,{fx[1]}}}, {{2,{fx[2]}}} }} ");

        // f(x) = ax^2 + bx + c
        // for x=1: a + b = x1 - c => b = x1 - a - c;
        // for x=2: 2a + b = (x2 - c) / 2 => 2a + x1 - a - c = (x2 - c) / 2
        var (x0, x1, x2) = (fx[0], fx[1], fx[2]);
        long c = x0;
        long a = (x2 + c - 2 * x1) / 2;
        long b = x1 - a - c;

        return (a * dv * dv) + (b * dv) + c;
    }

    private int Part1(List<string> input, int sx, int sy, int stepsTarget)
    {
        var seen = new HashSet<(long,long)> { (sx,sy) };
        var stepCounts = new List<long>();
        for (var steps = 0; steps < stepsTarget; steps++)
        {
            var tovisit = seen.ToList();
            seen.Clear();
            foreach (var (cx,cy) in tovisit)
            {
                var neigh = Neighbours(input, cx, cy, 1);
                foreach (var (nx,ny) in neigh)
                {
                    if (seen.Contains((nx,ny)))
                        continue;
                    seen.Add((nx,ny));
                }
            }
            stepCounts.Add(seen.Count);
        }
        return seen.Count;
    }

    private readonly (int,int)[] Moves = [(0,-1), (1,0), (0,1), (-1,0)];

    private (long,long)[] Neighbours(List<string> map, long x, long y, int part)
    {
        int LimX = map[0].Length - 1, LimY = map.Count - 1;
        int xr = LimX + 1, yr = LimY + 1;
        var result = new List<(long,long)>();
        foreach (var (dx, dy) in Moves)
        {
            var (px, py) = (x+dx,y+dy);
            if (part == 1 && (px < 0 || py < 0 || px > LimX || py > LimY || map[(int)py][(int)px] == '#'))
                continue;
            if (part == 2)
            {
                // NB: mod for -ve numbers in C# '-4 % 5 = -4' & '-9 % 5 = -4'
                var vx = ((px % xr) + xr) % xr;
                var vy = ((py % yr) + yr) % yr;
                if (map[(int)vy][(int)vx] == '#')
                    continue;
            }
            result.Add((px,py));
        }

        return result.ToArray();
    }
}
