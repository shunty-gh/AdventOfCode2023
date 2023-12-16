namespace Shunty.AoC;

// https://adventofcode.com/2023/day/16 - The Floor Will Be Lava

public class Day16 : AocDaySolver
{
    public int DayNumber => 16;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        int xLim = input[0].Length, yLim = input.Count;
        var map = BuildBeamTrajectories(input);

        var p1 = Energise(new(0,0,1), map, xLim, yLim);
        this.ShowDayResult(1, p1);

        var p2 =0;
        foreach (var (y,d) in new (int,int)[] { (0,2), (yLim-1,0) })
        {
            foreach (var x in Enumerable.Range(0, xLim))
            {
                var energised = Energise(new(x,y,d), map, xLim, yLim);
                if (p2 < energised)
                    p2 = energised;
            }
        }
        foreach (var (x,d) in new (int,int)[] { (0,1), (xLim-1,3) })
        {
            foreach (var y in Enumerable.Range(0, yLim))
            {
                var energised = Energise(new(x,y,d), map, xLim, yLim);
                if (p2 < energised)
                    p2 = energised;
            }
        }
        this.ShowDayResult(2, p2);
    }

    private int Energise(Beamstate start, IReadOnlyDictionary<Beamstate,Beamstate[]> map, int xLim, int yLim)
    {
        var visited = new HashSet<Beamstate>();
        var uniqueVisits = new HashSet<Pt>();
        var q = new Queue<Beamstate>();
        q.Enqueue(start);
        while (q.Count > 0)
        {
            var curr = q.Dequeue();
            while (curr.X >= 0 && curr.Y >= 0 && curr.X < xLim && curr.Y < yLim && !visited.Contains(curr))
            {
                visited.Add(curr);
                uniqueVisits.Add(new(curr.X,curr.Y));
                // Get next possible point(s). Will only ever be exactly 1 or 2.
                var nextposs = map[curr];
                curr = nextposs[0];
                if (nextposs.Length > 1)
                {
                    q.Enqueue(nextposs[1]);
                }
            }
        }
        return uniqueVisits.Count;
    }

    private record Beamstate(int X, int Y, int Direction);

    /// <summary>
    /// For each point in the initial map, pre-determine each subsequent exit
    /// point based on the content of the point (ie mirror or empty) and the
    /// direction of the beam entrypoint.
    /// </summary>
    private Dictionary<Beamstate,Beamstate[]> BuildBeamTrajectories(IList<string> input)
    {
        var result = new Dictionary<Beamstate, Beamstate[]>();
        for (var y = 0; y < input.Count; y++)
        {
            var row = input[y];
            for (var x = 0; x < row.Length; x++)
            {
                switch (row[x])
                {
                    // Empty space allows the beam to continue on the same direction
                    case '.':
                        result.Add(new(x,y,0), [new(x,y-1,0)]);
                        result.Add(new(x,y,1), [new(x+1,y,1)]);
                        result.Add(new(x,y,2), [new(x,y+1,2)]);
                        result.Add(new(x,y,3), [new(x-1,y,3)]);
                        break;
                    // '/' and '\' mirrors deflect by 90 degrees
                    case '/':
                        result.Add(new(x,y,0), [new(x+1,y,1)]);
                        result.Add(new(x,y,1), [new(x,y-1,0)]);
                        result.Add(new(x,y,2), [new(x-1,y,3)]);
                        result.Add(new(x,y,3), [new(x,y+1,2)]);
                        break;
                    case '\\':
                        result.Add(new(x,y,0), [new(x-1,y,3)]);
                        result.Add(new(x,y,1), [new(x,y+1,2)]);
                        result.Add(new(x,y,2), [new(x+1,y,1)]);
                        result.Add(new(x,y,3), [new(x,y-1,0)]);
                        break;
                    // Flat mirrors '-' and '|' will split the beam in two
                    case '-':
                        result.Add(new(x,y,0), [new(x-1,y,3),new(x+1,y,1)]);
                        result.Add(new(x,y,1), [new(x+1,y,1)]);
                        result.Add(new(x,y,2), [new(x-1,y,3),new(x+1,y,1)]);
                        result.Add(new(x,y,3), [new(x-1,y,3)]);
                        break;
                    case '|':
                        result.Add(new(x,y,0), [new(x,y-1,0)]);
                        result.Add(new(x,y,1), [new(x,y-1,0),new(x,y+1,2)]);
                        result.Add(new(x,y,2), [new(x,y+1,2)]);
                        result.Add(new(x,y,3), [new(x,y-1,0),new(x,y+1,2)]);
                        break;
                    default:
                        throw new Exception($"Unexpected item in the packing area, {row[x]}");
                }
            }
        }
        return result;
    }
}
