namespace Shunty.AoC;

// https://adventofcode.com/2023/day/23 - A Long Walk

/* Oh my. This gives the right result. Eventually.
   It is so slow and such a mess. But I have lost the will to continue with it so
   it can stay like this for the time being.
 */

public class Day23 : AocDaySolver
{
    public int DayNumber => 23;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);

        this.ShowDayResult(1, Part1(input));
        this.ShowDayResult(2, Part2(input));
    }

    private record NodePath(int Id, int X0, int Y0, int X1, int Y1, int Len);

    private int Part2(List<string> map)
    {
        // There's probably a lot of tidying up required here. But who cares. For now.

        var targetY = map.Count - 1;
        var targetX = map[targetY].IndexOf('.');
        var nodes = new List<NodePath>();
        var done = new HashSet<(int,int,int,int)>();
        var q = new Queue<(int,int,int,int)>();
        var id = 0;

        // Make a list of a segments/chunks. ie continue from a point until
        // there is a choice. Once we get to a choice start a new segment for
        // each choice a repeat the process.
        q.Enqueue((1,1,1,0));
        while (q.Count > 0)
        {
            var (cx,cy,fromx,fromy) = q.Dequeue();
            if (done.Contains((cx,cy,fromx,fromy)))
                continue;
            done.Add((cx,cy,fromx,fromy));
            var x0 = fromx;
            var y0 = fromy;
            var len = 1;
            while (true)
            {
                var neigh = Neighbours2(map, cx, cy, fromx, fromy);
                if (neigh.Count == 1)
                {
                    len += 1;
                    (fromx,fromy) = (cx,cy);
                    (cx,cy) = neigh[0];
                }
                else
                {
                    var np1 = nodes.Find(np => np.X0 == cx && np.Y0 == cy && np.X1 == x0 && np.Y1 == y0);
                    if (np1 is not null)
                    {
                        nodes.Add(new NodePath(-1 * np1.Id, x0, y0, cx, cy, len));
                    }
                    else
                    {
                        nodes.Add(new NodePath(id, x0, y0, cx, cy, len));
                    }
                    id += 1;
                    foreach (var (nx, ny) in neigh)
                    {
                        q.Enqueue((nx, ny, cx, cy));
                    }
                    break;
                }
            }
        }

        // Find all combinations of segments that lead us to the end. And make
        // sure we don't revisit any sgement start or end point. There's a lot.
        var paths = new List<List<int>>();
        var q2 = new Queue<(List<int>, HashSet<(int,int)>)>();
        q2.Enqueue(([0], [(1,0)]));
        var targetNd = nodes.First(n => n.X1 == targetX && n.Y1 == targetY);
        while (q2.Count > 0)
        {
            var (npath,ppath) = q2.Dequeue();
            var lastnode = nodes.First(n => n.Id == npath.Last());
            var (lastx,lasty) = (lastnode.X1, lastnode.Y1);
            var nxtpp = new HashSet<(int,int)>(ppath) { (lastx, lasty) };

            var choices = nodes.Where(n => n.X0 == lastx && n.Y0 == lasty);
            if (choices.Any(c => c.Id == targetNd.Id))
            {
                npath.Add(targetNd.Id);
                paths.Add(npath);
                continue;
            }
            foreach (var choice in choices)
            {
                if (nxtpp.Contains((choice.X1, choice.Y1)))
                    continue;
                if (npath.Contains(choice.Id))
                    continue;
                q2.Enqueue((new List<int>(npath) { choice.Id}, new HashSet<(int, int)>(nxtpp)));
            }
        }

        // For each path, find the longest
        var p2 = 0;
        foreach (var p in paths)
        {
            var pp2 = 0;
            foreach (var pn in p)
            {
                pp2 += nodes.First(n => n.Id == pn).Len;
            }
            if (pp2 > p2)
                p2 = pp2;
        }
        return p2;
    }

    private (int,int,int) Backtrack(List<string> map)
    {
        var targetY = map.Count - 1;
        var targetX = map[targetY].IndexOf('.');
        var (cx,cy) = (targetX,targetY);
        var (px,py) = (0,0);
        var (nx,ny) = (0,0);
        var steps = 0;
        var neigh = Neighbours(map, cx, cy, 2);
        while (neigh.Count <= 2)
        {
            steps += 1;
            if ((px,py) == neigh[0])
                (nx,ny) = neigh[1];
            else
                (nx,ny) = neigh[0];
            (px,py) = (cx,cy);
            (cx,cy) = (nx,ny);
            neigh = Neighbours(map, cx, cy, 2);
        }
        return (cx,cy,steps);
    }

    private int Part1(List<string> map)
    {
        var targetY = map.Count - 1;
        var targetX = map[targetY].IndexOf('.');
        var p1 = 0;
        var q = new Queue<QueueItem>();
        q.Enqueue(new(1,0,new List<(int,int)>()));
        while (q.Count > 0)
        {
            var (cx,cy,cp) = q.Dequeue();
            var neigh = Neighbours(map, cx, cy);
            foreach (var (nx,ny) in neigh)
            {
                if (nx == targetX && ny == targetY)
                {
                    var plen = cp.Count + 1;
                    if (plen > p1)
                    {
                        p1 = plen;
                    }
                    continue;
                }
                if (!cp.Contains((nx,ny)))
                {
                    var npath = new List<(int,int)>(cp);
                    npath.Add((nx,ny));
                    q.Enqueue(new(nx, ny, npath));
                }
            }
        }
        return p1;
    }

    private readonly Pt[] DirectionDelta = [new(0,-1), new(1,0), new(0,1), new(-1,0)];
    private readonly char[] Hills = ['^', '>', 'v', '<'];

    private bool InMap(List<string> map, int x, int y) =>
        x >= 0 && y >= 0 && x < map[0].Length && y < map.Count;

    private List<(int,int)> Neighbours2(List<string> map, int x, int y, int fromx, int fromy)
    {
        var n = Neighbours(map, x, y, 2);
        return n.Where(nn => !(nn.Item1 == fromx && nn.Item2 == fromy)).ToList();
    }

    private List<(int,int)> Neighbours(List<string> map, int x, int y, int part = 1)
    {
        var result = new List<(int,int)>();
        if (part == 1)
        {
            var curr = map[y][x];
            var hi = Array.IndexOf(Hills, curr);
            if (hi >= 0)
            {
                var (hx,hy) = DirectionDelta[hi];
                result.Add((x+hx,y+hy));
                return result;
            }
        }
        foreach (var (dx,dy) in DirectionDelta)
        {
            var (nx,ny) = (x+dx,y+dy);
            if (InMap(map, nx,ny) && map[ny][nx] != '#')
            {
                result.Add((nx,ny));
            }
        }
        return result;
    }

    private record QueueItem(int X, int Y, List<(int,int)> Path);
}
