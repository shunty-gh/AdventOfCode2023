using System.Drawing;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/14 - Parabolic Reflector Dish

public class Day14 : AocDaySolver
{
    public int DayNumber => 14;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var mapBounds = new Rectangle(0, 0, input[0].Length, input.Count);
        var map = new List<char[]>();
        foreach (var line in input)
        {
            map.Add(line.ToCharArray());
        }

        // Part 1
        var m2 = TiltNorth(map, mapBounds);
        var p1 = GetLoad(m2, mapBounds);
        this.ShowDayResult(1, p1);

        // Part 2
        // Rather than run 1,000,000,000 cycles we should run "a few" and store
        // the "load" value after each cycle. Then check for a repeating
        // sequence of loads. Once we find the cycle we can predict the load for
        // the full set of cycles.
        var loads = new List<int>();
        var cyclesToRun = 400; // Not 1_000_000_000! but can go lower than 400 if we really want
        var m = TiltCycle(map, mapBounds);
        loads.Add(GetLoad(m, mapBounds));
        for (var c = 1; c < cyclesToRun; c++)
        {
            m = TiltCycle(m, mapBounds);
            loads.Add(GetLoad(m, mapBounds));
        }

        var seq = loads.FindRepeatingSequence();
        // Check we have found a sequence. If not then we need to increase the cyclesToRun variable above
        if (seq.StartIndex < 0 || seq.SequenceLength < 0)
            throw new Exception("Repeating sequence not found in 'load' values. Try increasing the sample size.");
        var targetCycles = 1_000_000_000;
        var dr = Math.DivRem(targetCycles - seq.StartIndex, seq.SequenceLength);
        var loadIndex = seq.StartIndex + seq.SequenceLength + dr.Remainder - 1; // Add seq.SequenceLength to allow for remainder of 0, just in case
        var p2 = loads[loadIndex];
        this.ShowDayResult(2, p2);
    }

    private static List<char[]> TiltCycle(List<char[]> map, Rectangle bounds)
    {
        var n = TiltNorth(map, bounds);
        var n1 = TiltSouth(n, bounds);
        var n2 = TiltNorth(n1, bounds);
        var w = TiltWest(n, bounds);
        var s = TiltSouth(w, bounds);
        var e = TiltEast(s, bounds);
        return e;
    }

    /// <summary>
    /// In place 'tilt' function to move all 'O' characters to the front end
    /// of the given array unless blocked by '#' characters.
    /// </summary>
    private static void Tilt(char[] colOrRow)
    {
        int firstEmpty;
        for (var y = 0; y < colOrRow.Length; y++)
        {
            // Scan for first empty cell
            var c = colOrRow[y];
            if (c != '.')
                continue;
            firstEmpty = y;

            // Scan for rock or roller
            for (var yy = y+1; yy < colOrRow.Length; yy++)
            {
                var r = colOrRow[yy];
                if (r == '#')
                {
                    y = yy;
                    break;
                }
                else if (r == 'O')
                {
                    colOrRow[firstEmpty] = 'O';
                    colOrRow[yy] = '.';
                    y = firstEmpty;
                    break;
                }
            }
        }
    }

    private static List<char[]> TiltNorth(List<char[]> map, Rectangle bounds) =>
        TiltVertical(map, bounds, true);

    private static List<char[]> TiltSouth(List<char[]> map, Rectangle bounds) =>
        TiltVertical(map, bounds, false);

    private static List<char[]> TiltWest(List<char[]> map, Rectangle bounds) =>
        TiltHorizontal(map, bounds, true);

    private static List<char[]> TiltEast(List<char[]> map, Rectangle bounds) =>
        TiltHorizontal(map, bounds, false);

    private static List<char[]> TiltVertical(List<char[]> map, Rectangle bounds, bool tiltNorth)
    {
        var columns = new List<char[]>();
        for (var x = bounds.Left; x < bounds.Width; x++)
        {
            if (tiltNorth)
            {
                var col = map.Select(c => c[x]).ToArray();
                Tilt(col);
                columns.Add(col);
            }
            else
            {
                var col = map.Select(c => c[x]).Reverse().ToArray();
                Tilt(col);
                columns.Add(col.Reverse().ToArray());
            }
        }
        // Flip the cols back to rows
        var result = new List<char[]>();
        for (var y = 0; y < bounds.Height; y++)
        {
            result.Add(columns.Select(c => c[y]).ToArray());
        }

        return result;
    }

    private static List<char[]> TiltHorizontal(List<char[]> map, Rectangle bounds, bool tiltWest)
    {
        var rows = new List<char[]>();
        for (var y = bounds.Top; y < bounds.Height; y++)
        {
            if (tiltWest)
            {
                var row = map[y].ToArray();
                Tilt(row);
                rows.Add(row);
            }
            else
            {
                var row = map[y].Reverse().ToArray();
                Tilt(row);
                rows.Add(row.Reverse().ToArray());
            }
        }
        return rows;
    }

    private static int GetLoad(List<char[]> map, Rectangle bounds)
    {
        int result = 0;
        for (int y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (int x = bounds.Left; x < bounds.Width; x++)
            {
                if (map[y][x] == 'O')
                {
                    result += bounds.Height - y;
                }
            }
        }
        return result;;
    }
}
