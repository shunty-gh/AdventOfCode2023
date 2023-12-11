namespace Shunty.AoC;

// https://adventofcode.com/2023/day/10 - Pipe Maze

public class Day10 : AocDaySolver
{
    public int DayNumber => 10;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test1");
        var input = await AocUtils.GetDayLines(DayNumber);
        (int LimX, int LimY) bounds = (input[0].Length, input.Count);

        var (start, startPipe, firstMove) = FindStart(input);
        //Console.WriteLine($"Start {start}; Start pipe {startPipe}; First move to {firstMove}");

        // P1
        int steps = 1;
        var current = firstMove;
        var path = new HashSet<Pt> { start };
        Pt from = start;
        while (true)
        {
            steps++;
            path.Add(current);
            var nx = Next(input, current, from);
            if (nx == start)
                break;
            from = current;
            current = nx;
        }

        var p1 = steps / 2;
        if (steps % 2 != 0)
            p1 += 1;
        this.ShowDayResult(1, p1);

        // P2
        // Read left to right, top to bottom
        // Each time we cross a path character check whether to flip
        // the inside/outside state marker.
        // Always change on '|', never on '-'.
        // Due to reading L to R we can never come across a 'J' or '7' unless
        // we are already 'on the path'.
        // If we last met an F then a J will cause the state to stay the same whereas a 7 will cause the sate to flip
        // Similarly for an L followed (after one or more '-') by J or 7
        var incount = 0;
        var outcount = 0;
        char lastcorner = ' ';
        // var outside = new Dictionary<Pt, int>();
        // var inside = new Dictionary<Pt, int>();
        for (var y = 0; y < bounds.LimY; y++)
        {
            var state = 0;
            for (var x = 0; x < bounds.LimX; x++)
            {
                var curr = new Pt(x,y);
                var c = input[y][x];
                if (c == 'S')
                    c = startPipe;
                if (path.Contains(curr))
                {
                    switch (c)
                    {
                        case '|':
                            state = state == 0 ? 1 : 0;
                            lastcorner = ' ';
                            break;
                        case 'F':
                        case 'L':
                            lastcorner = c == 'S' ? 'F' : c;
                            state = state == 0 ? 1 : 0;
                            break;
                        case 'J':
                            if (lastcorner != 'F')
                                state = state == 0 ? 1 : 0;
                            break;
                        case '7':
                            if (lastcorner != 'L')
                                state = state == 0 ? 1 : 0;
                            break;
                        case '-':
                            break;
                    }
                }
                else
                {
                    if (state == 1)
                    {
                        // inside.Add(curr, 0);
                        incount++;
                    }
                    else
                    {
                        // outside.Add(curr, 0);
                        outcount++;
                    }
                }
            }
        }

        // this.ShowDayResult(2, bounds.LimX * bounds.LimY);
        // this.ShowDayResult(2, path.Count);
        // this.ShowDayResult(2, outcount);
        this.ShowDayResult(2, incount);
    }

    private Pt Next(IList<string> map, Pt current, Pt cameFrom)
    {
        var pipe = map[current.Y][current.X];
        Pt result = pipe switch
        {
            'F' => cameFrom.X == current.X ? new(current.X+1, current.Y) : new(current.X, current.Y+1),
            'J' => cameFrom.X == current.X ? new(current.X-1, current.Y) : new(current.X, current.Y-1),
            'L' => cameFrom.X == current.X ? new(current.X+1, current.Y) : new(current.X, current.Y-1),
            '7' => cameFrom.X == current.X ? new(current.X-1, current.Y) : new(current.X, current.Y+1),
            '|' => cameFrom.Y > current.Y ? new(current.X, current.Y-1) : new(current.X, current.Y+1),
            '-' => cameFrom.X > current.X ? new(current.X-1, current.Y) : new(current.X+1, current.Y),
            _ => throw new Exception($"Unknown pipe segment [{pipe}]"),
        };
        return result;
    }

    private (Pt start, char startPipe, Pt firstMove) FindStart(IList<string> map)
    {
        Pt start = new(0,0);
        for (var y = 0; y < map.Count; y++)
        {
            for (var x = 0; x < map[0].Length; x++)
            {
                if (map[y][x] == 'S')
                {
                    start = new(x, y);
                    break;
                }
            }
            if (start.X > 0)
                break;
        }
        var (pipe, firstMove) = FindStartPipeAndFirstMove(map, start);
        return (start, pipe, firstMove);
    }

    private (char, Pt) FindStartPipeAndFirstMove(IList<string> map, Pt start)
    {
        Pt[] delta = [new(0,-1), new(1,0), new(0,1), new(-1,0)];

        Pt? firstMove = default;
        var outindex = -1;
        var inindex = -1;
        var index = 0;
        foreach (var (dx, dy) in delta)
        {
            Pt p = new(start.X+dx, start.Y+dy);
            if (p.X < 0 || p.Y < 0 || p.X >= map[0].Length || p.Y >= map.Count)
                continue;

            var c = map[p.Y][p.X];
            if ((index == 0 && UpChars.Contains(c))
                || (index == 1 && RightChars.Contains(c))
                || (index == 2 && DownChars.Contains(c))
                || (index == 3 && LeftChars.Contains(c)))
            {
                if (outindex < 0)
                {
                    firstMove = p;
                    outindex = index;
                }
                else
                {
                    inindex = index;
                    break;
                }
            }

            index++;
        }

        char pipe;
        if (outindex == 0) // up. start must be J,L,|
            pipe = inindex switch
            {
                1 => 'L',
                2 => '|',
                3 => 'J',
                _ => ' ',
            };
        else if (outindex == 1) // right. start must be F,-
            pipe = inindex == 2 ? 'F' : '-';
        else //if (outindex == 2) // down. start must be 'in left', 'out down', ie 7
            pipe = '7';

        return (pipe, firstMove!);
    }

    private static readonly char[] UpChars = ['F', '7', '|'];
    private static readonly char[] DownChars = ['J', 'L', '|'];
    private static readonly char[] LeftChars = ['F', 'L', '-'];
    private static readonly char[] RightChars = ['J', '7', '-'];
}
