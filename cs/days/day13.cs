namespace Shunty.AoC;

// https://adventofcode.com/2023/day/13 - Point of Incidence

public class Day13 : AocDaySolver
{
    public int DayNumber => 13;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var blocks = new List<List<char[]>> { new() };
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                blocks.Add([]);
                continue;
            }
            blocks[^1].Add(line.ToCharArray());
        }

        // Part 1
        var mirrorScores = new Dictionary<int, int>();
        var p1 = 0;
        var bi = 0;
        foreach (var block in blocks)
        {
            var score = GetBlockScore(block);
            // Save scores for part 2
            mirrorScores[bi] = score;
            p1 += score;
            bi++;
        }
        this.ShowDayResult(1, p1);

        // Part 2
        var p2 = 0;
        bi = 0;
        foreach (var block in blocks)
        {
            // Get the original score so we can skip it when searching
            var orgMI = mirrorScores[bi];

            var found = false;
            Pt prev = new(-1, -1), curr = new(0, 0);
            for (var y = 0; y <= block.Count - 1; y++)
            {
                for (var x = 0; x <= block[0].Length - 1; x++)
                {
                    curr = new(x,y);
                    ChangeBlockElement(block, curr, prev);
                    var score = GetBlockScore(block, orgMI);
                    if (score > 0)
                    {
                        found = true;
                        p2 += score;
                        break;
                    }
                    prev = curr;
                }
                if (found)
                    break;
            }
            bi++;
        }
        this.ShowDayResult(2, p2);
    }

    private int GetBlockScore(List<char[]> block, int skipScore = -1)
    {
        Func<int, IEnumerable<char>> rowFunc = (i) => block[i];
        Func<int, IEnumerable<char>> colFunc = (i) => block.Select(c => c[i]);

        var result = GetRCScore(block.Count - 1, rowFunc, 100, skipScore);
        if (result <= 0)
        {
            result = GetRCScore(block[0].Length - 1, colFunc, 1, skipScore);
        }
        return result;
    }

    private int GetRCScore(int max, Func<int, IEnumerable<char>> fn, int mult, int skipScore = -1)
    {
        for (var i = 1; i <= max; i++)
        {
            if (fn(i).SequenceEqual(fn(i - 1)) && (i * mult) != skipScore)
            {
                // We now need to check that each row or column either side is also mirrored
                var allmatch = true;
                for (var (hi,lo) = (i+1,i-2); hi <= max && lo >= 0; hi++, lo--)
                {
                    if (!fn(hi).SequenceEqual(fn(lo)))
                    {
                        allmatch = false;
                        break;
                    }
                }
                if (allmatch)
                    return i * mult;
            }
        }
        return 0;
    }

    private void ChangeBlockElement(List<char[]> block, Pt toChange, Pt prev)
    {
        // Change previous element back to what it was
        if (prev.X >= 0 && prev.Y >= 0)
        {
            var (px,py) = prev;
            block[py][px] = block[py][px] == '.' ? '#' : '.';
        }
        // Change the indexed element
        var (cx,cy) = toChange;
        block[cy][cx] = block[cy][cx] == '.' ? '#' : '.';
    }

}
