namespace Shunty.AoC;

// https://adventofcode.com/2023/day/17

public class Day17 : AocDaySolver
{
    public int DayNumber => 17;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var zero = (int)'0';
        var map = new List<List<int>>();
        foreach (var line in input)
        {
            map.Add(new List<int>(line.Select(c => (int)c - zero)));
        }

        int targetX = map[0].Count - 1, targetY = map.Count - 1;
        var visited = new Dictionary<VisitedKey, int>();
        var q = new PriorityQueue<QueueItem, int>();
        q.Enqueue(new(0, 0, 0, 1, 0), 0);
        var bestCost = int.MaxValue;
        while (q.Count > 0)
        {
            var curr = q.Dequeue();
            if (curr.Cost > bestCost)
                continue;
            var vkey = new VisitedKey(curr.X, curr.Y, curr.Dir, curr.DirCount);
            if (visited.ContainsKey(vkey) && visited[vkey] < curr.Cost)
                continue;
            visited[vkey] = curr.Cost;

            var visitable = GetVisitableNeighbours(map, curr);
            foreach (var n in visitable)
            {
                if (n.Cost >= bestCost)
                    continue;
                if (n.X == targetX && n.Y == targetY)
                {
                    if (n.Cost < bestCost)
                    {
                        bestCost = n.Cost;
                    }
                    continue;
                }
                var nkey = new VisitedKey(n.X, n.Y, n.Dir, n.DirCount);
                if (visited.ContainsKey(nkey))
                {
                    if (visited[nkey] > n.Cost)
                    {
                        visited[nkey] = n.Cost;
                        q.Enqueue(n, n.Cost);
                    }
                }
                else
                {
                    q.Enqueue(n, n.Cost);
                }
            }
        }
        this.ShowDayResult(1, bestCost);
        this.ShowDayResult(2, 0);
    }

    private readonly Pt[] DirectionDelta = [new(0,-1), new(1,0), new(0,1), new(-1,0)];

    private List<QueueItem> GetVisitableNeighbours(List<List<int>> map, QueueItem current)
    {
        var result = new List<QueueItem>();
        Pt[] adjacent = [
            new(current.X + DirectionDelta[0].X, current.Y + DirectionDelta[0].Y),
            new(current.X + DirectionDelta[1].X, current.Y + DirectionDelta[1].Y),
            new(current.X + DirectionDelta[2].X, current.Y + DirectionDelta[2].Y),
            new(current.X + DirectionDelta[3].X, current.Y + DirectionDelta[3].Y),
        ];
        int[] choices;
        switch (current.Dir)
        {
            case 0:
                choices = [0,1,3];
                break;
            case 1:
                choices = [0,1,2];
                break;
            case 2:
                choices = [1,2,3];
                break;
            case 3:
                choices = [0,2,3];
                break;
            default:
                throw new Exception($"Unexpected direction {current.Dir}");
        }

        foreach (var choice in choices)
        {
            var adj = adjacent[choice];
            if (choice == current.Dir && current.DirCount == 3)
                continue;
            if (adj.X < 0 || adj.Y < 0 || adj.X >= map[0].Count || adj.Y >= map.Count)
                continue;
            var dirc = choice == current.Dir ? current.DirCount + 1 : 1;
            var stepcost = map[adj.Y][adj.X];
            result.Add(new(adj.X, adj.Y, current.Cost + stepcost, choice, dirc));
        }
        return result;
    }

    private record QueueItem(int X, int Y, int Cost, int Dir, int DirCount);
    private record VisitedKey(int X, int Y, int Dir, int DirCount);
}
