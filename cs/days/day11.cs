namespace Shunty.AoC;

// https://adventofcode.com/2023/day/11 - Cosmic Expansion

public class Day11 : AocDaySolver
{
    public int DayNumber => 11;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var map = new List<int[]>();
        var colsToExpand = new List<int>();
        var rowsToExpand = new List<int>();
        BuildMap(input, map, rowsToExpand,colsToExpand);

        var m1 = ExpandMap(map, rowsToExpand, colsToExpand);
        var pathlengths = GetPathLengths(m1);
        this.ShowDayResult(1, pathlengths.Sum());

        var m2 = ExpandMap(map, rowsToExpand, colsToExpand, 1000000-1);
        pathlengths = GetPathLengths(m2);
        Int64 p2 = pathlengths.Sum64();
        this.ShowDayResult(2, p2);
    }

    private void BuildMap(IList<string> input, List<int[]> map, List<int> rowsToExpand, List<int> colsToExpand)
    {
        var xlen = input[0].Length;
        var ylen = input.Count;
        // Find galaxies and empty rows
        for (var y = 0; y < ylen; y++)
        {
            var row = input[y];
            var empty = true;
            for (var x = 0; x < xlen; x++)
            {
                if (row[x] == '#')
                {
                    map.Add([x,y]);
                    empty = false;
                }
            }
            if (empty)
                rowsToExpand.Add(y);
        }
        // Find empty cols
        for (var x = 0; x < xlen; x++)
        {
            var empty = true;
            for (var y = 0; y < ylen; y++)
            {
                if (input[y][x] == '#')
                {
                    empty = false;
                    break;
                }
            }
            if (empty)
                colsToExpand.Add(x);
        }
    }

    private List<int[]> ExpandMap(List<int[]> map, List<int> emptyRows, List<int> emptyCols, int by = 1)
    {
        var result = map.Select(m => new int[] { m[0], m[1] } ).ToList();
        for (int ci = emptyCols.Count-1; ci >= 0; ci--)
        {
            var c = emptyCols[ci];
            foreach (var el in result)
            {
                if (el[0] > c)
                    el[0] += by;
            }
        }
        for (int ri = emptyRows.Count-1; ri >= 0; ri--)
        {
            var r = emptyRows[ri];
            foreach (var el in result)
            {
                if (el[1] > r)
                    el[1] += by;
            }
        }
        return result;
    }

    /// <summary>
    /// Get the Manhattan distance between all pairs of co-ords in the map
    /// </summary>
    private List<int> GetPathLengths(List<int[]> map)
    {
        var result = new List<int>();
        for (var i = 0; i < map.Count - 1; i++)
        {
            var e1 = map[i];
            for (var j = i + 1; j < map.Count; j++)
            {
                var e2 = map[j];
                result.Add(Math.Abs(e1[0] - e2[0]) + Math.Abs(e1[1] - e2[1]));
            }
        }
        return result;
    }
}
