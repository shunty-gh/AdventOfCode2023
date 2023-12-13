using System.Collections.ObjectModel;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/13 - Point of Incidence

public class Day13 : AocDaySolver
{
    public int DayNumber => 13;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var blocks = ProcessInput(input.AsReadOnly());

        // Part 1
        var mirrorInfo = new Dictionary<int, MirrorInfo>();
        var p1 = 0;
        var bi = 0;
        foreach (var block in blocks)
        {
            var (rowscores, colscores) = GetBlockScores(block);
            var mi = GetMirrorInfo(rowscores.AsReadOnly(), colscores.AsReadOnly());
            p1 += mi.Score;
            // Save it for part 2
            mirrorInfo.Add(bi, mi);
            bi++;
        }
        this.ShowDayResult(1, p1);

        // Part 2
        var p2 = 0;
        bi = 0;
        foreach (var block in blocks)
        {
            // Get the original mirror info so we can skip it when searching
            var orgMI = mirrorInfo[bi];

            var blockscore = 0;
            var found = false;
            Pt prev = new(-1, -1), curr = new(0, 0);
            for (var y = 0; y <= block.Count - 1; y++)
            {
                for (var x = 0; x <= block[0].Length - 1; x++)
                {
                    curr = new(x,y);
                    ChangeBlockElement(block, curr, prev);
                    var (rowscores, colscores) = GetBlockScores(block);
                    var mi = GetMirrorInfo(rowscores.AsReadOnly(), colscores.AsReadOnly(), orgMI);
                    if (mi.IsValid)
                    {
                        found = true;
                        blockscore = mi.Score;
                        break;
                    }
                    prev = curr;
                }
                if (found)
                    break;
            }
            p2 += blockscore;
            bi++;
        }
        this.ShowDayResult(2, p2);
    }

    private List<List<char[]>> ProcessInput(IReadOnlyCollection<string> input)
    {
        var result = new List<List<char[]>>();
        var block = new List<char[]>();
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (block.Count > 0)
                {
                    result.Add(block);
                    block = [];
                }
                continue;
            }
            block.Add(line.ToCharArray());
        }
        if (block.Count > 0)
        {
            result.Add(block);
        }

        return result;
    }

    /// <summary>
    /// Create a 'score' for each row and column of the block. A simple bit shifted
    /// integer where '#' == 1 and '.' == 0.
    /// <para />This makes it easy to compare rows and cols just by their score rather
    /// than string comparison.
    /// </summary>
    private (int[], int[]) GetBlockScores(IReadOnlyList<char[]> block)
    {
        var h = block.Count;
        var w = block[0].Length;
        var rowscores = new int[h];
        var colscores = new int[w];
        for (var y = 0; y <= h-1; y++)
        {
            for (var x = 0; x <= w-1; x++)
            {
                var c = block[y][x];
                rowscores[y] *= 2;
                rowscores[y] += c == '#' ? 1 : 0;

                colscores[x] *= 2;
                colscores[x] += c == '#' ? 1 : 0;
            }
        }
        return (rowscores, colscores);
    }

    /// <summary>
    /// Compare adjacent pairs of rows and pairs of columns to find a match and
    /// then check that all surrounding rows or cols also pair off.
    /// </summary>
    private MirrorInfo GetMirrorInfo(ReadOnlyCollection<int> rowScores, ReadOnlyCollection<int> colScores, MirrorInfo? skip = null)
    {
        // Look at the rows first
        var result = skip is not null && skip.IsRow
            ? GetMirrorLine(rowScores, skip.Index)  // P2
            : GetMirrorLine(rowScores);                  // P1

        if (result >= 0)
            return new MirrorInfo(result, MirrorLineOrientation.Row);

        // ...otherwise, try the columns
        result = skip is not null && !skip.IsRow
            ? GetMirrorLine(colScores, skip.Index)  // P1
            : GetMirrorLine(colScores);                  // P2

        if (result >= 0)
            return new MirrorInfo(result, MirrorLineOrientation.Column);

        // Else, return an invalid MirrorInfo object
        return new MirrorInfo(-1, MirrorLineOrientation.Unknown);
    }

    private int GetMirrorLine(ReadOnlyCollection<int> scores, int skipIndex = -1)
    {
        int smax = scores.Count - 1;
        var lastseen = -1;
        for (var i = 0; i <= smax; i++)
        {
            var sc = scores[i];
            if (sc == lastseen && i != skipIndex)
            {
                // We now need to check that each row or column either side is also mirrored
                var allmatch = true;
                // Add the indexes of the initial matching pair together to help work out lower bounds
                var mm = i + (i-1);
                for (var hi = i+1; hi <= smax; hi++)
                {
                    var lo = mm - hi;
                    if (lo < 0) // matched all we can
                        break;

                    if (scores[hi] != scores[lo])
                    {
                        allmatch = false;
                        break;
                    }
                }
                if (allmatch)
                    return i;

            }
            lastseen = sc;
        }
        // We haven't found a mirror
        return -1;
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

    private enum MirrorLineOrientation { Unknown, Row, Column, }
    private record MirrorInfo(int Index, MirrorLineOrientation Orientation)
    {
        public bool IsRow =>
            Orientation == MirrorLineOrientation.Row;
        public bool IsValid =>
            Orientation != MirrorLineOrientation.Unknown && Index >= 0;
        public int Score =>
            Orientation == MirrorLineOrientation.Row
                ? Index * 100
                : Index;
    }
}
