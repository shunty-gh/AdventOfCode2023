using System.Globalization;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/18 - Lavaduct Lagoon

/*
 The sample data. Helping to get my brain around internal areas vs the
 '#' blocks on the AoC page.

 . . . . . . .

 ._._._._._._.
 |x x x x x x|
 . . . . . . .
 |x x x x x x|
 ._._. . . . .
     |x x x x|
 . . . . . . .
     |x x x x|
 . . . . . . .
     |x x x x|
 ._._. . ._._.
 |x x x x|
 . . . . . . .
 |x x x x|
 ._. . . ._._.
   |x x x x x|
 . . . . . . .
   |x x x x x|
 . ._._._._._.

 . . . . . . .

 */

public class Day18 : AocDaySolver
{
    public int DayNumber => 18;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var instructions = new List<Instruction>();
        foreach (var line in input)
        {
            var sp = line.Split(' ');
            instructions.Add(new(sp[0][0], int.Parse(sp[1]), sp[2].Trim(['(', ')', '#'])));
        }
        // This is what worked for part 1
        //var map = BuildMap(instructions);
        //this.ShowDayResult(1, GetMapAreaOriginalPart1Attempt(map));
        // But this is the improved Part 2 version applied to Part 1
        this.ShowDayResult(1, GetMapArea(instructions));

        // Part 2
        // Get a new instruction set based on the colour string
        var instructionsP2 = new List<Instruction>();
        foreach (var instruction in instructions)
        {
            var c = instruction.Colour;
            var dist = int.Parse(c[..^1], NumberStyles.HexNumber);
            var dir = c[^1] switch
            {
                '0' => 'R',
                '1' => 'D',
                '2' => 'L',
                '3' => 'U',
                _ => throw new Exception($"Unexpected direction flag {c[^1]}"),
            };
            instructionsP2.Add(new(dir, dist, c));
        }
        this.ShowDayResult(2, GetMapArea(instructionsP2));
    }


    private Int64 GetMapArea(List<Instruction> map)
    {
        /// The original part 1 solution method will get overwhelmed due to the
        /// massive numbers of points involved. Therefore we need a new approach:
        /// * Hit the Wikipedia...
        /// * Find out about Shoelace Theorem and Trapezoid formula
        ///   https://en.wikipedia.org/wiki/Shoelace_formula
        /// * Eventually realise that Shoelace only calculates the *interior* area
        ///   and we need to add the exterior path too because the perimeter
        ///   points in this use case contribute 1 unit each.
        /// * Discover Pick's Theorem
        ///   https://en.wikipedia.org/wiki/Pick%27s_theorem
        /// * Apply Pick's then discover an off by +1/-1 error
        /// * Then realise that we need to include the external corners of the
        ///   perimter but not double count the internal corners. But because
        ///   it's a closed loop there has to be exactly four more external
        ///   than internal corners. So add 4 to the perimeter count before
        ///   applying Pick's Theorem.

        Int64 perimeter = 4;
        Int64 result = 0;
        Int64 x = 0, y = 0;
        foreach (var instruction in map)
        {
            Int64 xi = x;
            Int64 yi = y;
            Int64 xj = instruction.Direction switch
            {
                'R' => xi + instruction.Distance,
                'L' => xi - instruction.Distance,
                _ => xi,
            };
            Int64 yj = instruction.Direction switch
            {
                'U' => yi - instruction.Distance,
                'D' => yi + instruction.Distance,
                _ => yi,
            };
            // Either: (Trapeziod theorem)
            result += (yi + yj) * (xi - xj);
            // Or: (Shoelace)
            //result += (xi * yj) - (xj * yi);
            x = xj;
            y = yj;
            perimeter += instruction.Distance;
        }
        // Pick's: Int area  +     P / 2       - 1
        //            v              v           v
        return (result / 2L) + (perimeter / 2) - 1;
    }

    [Obsolete("The original version when ony Part 1 mattered. Not needed for the Part 2 rewrite")]
    private Dictionary<Pt, string> BuildMap(List<Instruction> instructions)
    {
        var map = new Dictionary<Pt, string>();
        var current = new Pt(0,0);
        foreach (var instruction in instructions)
        {
            switch (instruction.Direction)
            {
                case 'U':
                    foreach (var y in Enumerable.Range(0, instruction.Distance))
                    {
                        map.Add(new(current.X, current.Y - y), instruction.Colour);
                    }
                    current = new(current.X, current.Y - instruction.Distance);
                    break;
                case 'D':
                    foreach (var y in Enumerable.Range(0, instruction.Distance))
                    {
                        map.Add(new(current.X, current.Y + y), instruction.Colour);
                    }
                    current = new(current.X, current.Y + instruction.Distance);
                    break;
                case 'L':
                    foreach (var x in Enumerable.Range(0, instruction.Distance))
                    {
                        map.Add(new(current.X - x, current.Y), instruction.Colour);
                    }
                    current = new(current.X - instruction.Distance, current.Y);
                    break;
                case 'R':
                    foreach (var x in Enumerable.Range(0, instruction.Distance))
                    {
                        map.Add(new(current.X + x, current.Y), instruction.Colour);
                    }
                    current = new(current.X + instruction.Distance, current.Y);
                    break;
                default:
                    throw new Exception($"Unexpected drilling direction {instruction.Direction}");
            }
        }
        return map;
    }

    [Obsolete("The original version when ony Part 1 mattered. Replaced by GetMapArea() method in Part 2")]
    private Int64 GetMapAreaOriginalPart1Attempt(Dictionary<Pt, string> map)
    {
        Int64 result = 0;
        var keys = map.Keys;
        var xmin = keys.Min(k => k.X)-1;
        var ymin = keys.Min(k => k.Y)-1;
        var xmax = keys.Max(k => k.X);
        var ymax = keys.Max(k => k.Y);
        for (var y = ymin; y <= ymax; y++)
        {
            bool state = false;
            for (var x = xmin; x <= xmax; x++)
            {
                var curr = new Pt(x,y);
                if (map.ContainsKey(curr))
                {
                    result += 1;
                    var up = new Pt(x,y-1);
                    var down = new Pt(x,y+1);
                    var right = new Pt(x+1,y);
                    if (map.ContainsKey(down))
                    {
                        state = !state;
                    }
                    else if (map.ContainsKey(up))
                    {
                        // stay same
                    }
                    else if (map.ContainsKey(right))
                    {
                        // stay same
                    }
                    else
                    {
                        state = !state;
                    }
                }
                else
                {
                    if (state)
                    {
                        result += 1;
                    }
                }
            }
        }
        return result;
    }

    private record Instruction(char Direction, int Distance, string Colour);
}
