namespace Shunty.AoC;

// https://adventofcode.com/2023/day/22 - Sand Slabs

/* Naive, literal version. Takes about 6+mins to run.
   Needs restructuring to sort by Z-index to reduce number of checks that
   need to be made. Maybe one day...
*/

public class Day22 : AocDaySolver
{
    public int DayNumber => 22;

    public async Task Solve()
    {
        //var input = await AocUtils.GetDayLines(DayNumber, "test");
        var input = await AocUtils.GetDayLines(DayNumber);
        var blocks = input.Select(ln => Block.Create(ln)).ToList();

        var bd = blocks.Select((b,i) => (i,b))
            .ToDictionary();
        AllFallDown(bd);

        this.ShowDayResult(1, Part1(bd));
        this.ShowDayResult(2, Part2(bd));
    }

    private int Part1(Dictionary<int, Block> blocks)
    {
        // Which of the blocks could be removed with nothing falling down
        int result = 0;
        foreach (var i in blocks.Keys)
        {
            var block = blocks[i];
            var canremove = true;
            // Check that no other block can fall if this one is removed
            // A block can only fall if it is currently being supported by  the
            // one to remove
            foreach (var j in blocks.Where(kvp => kvp.Value.Z0 > block.Z0 && kvp.Value.Z0 < block.Z1 + 2).Select(kvp => kvp.Key))
            {
                if (i ==j)
                    continue;
                var blocktocheck = blocks[j];
                var candrop = true;
                if (blocktocheck.CanDrop)
                {
                    var dropper = blocktocheck.Drop();
                    if (dropper is null)
                        continue;
                    foreach (var (k,b) in blocks)
                    {
                        if (k == i || k == j)
                            continue;
                        if (b.OverlapsWith(dropper))
                        {
                            candrop = false;
                            break;
                        }
                    }
                    if (candrop)
                    {
                        canremove = false;
                        break;
                    }
                }

            }
            if (canremove)
                result += 1;
        }

        return result;
    }

    private int Part2(Dictionary<int, Block> blocks)
    {
        // Which blocks will fall if we remove a block
        var fallen = new List<int>();
        foreach (var i in blocks.Keys)
        {
            // Make sure to use a fresh set each time
            var blockstotest = new Dictionary<int, Block>(blocks);
            var fell = AllFallDown(blockstotest, i);
            fallen.Add(fell);
        }

        return fallen.Sum();
    }

    private int AllFallDown(Dictionary<int, Block> blocks, int ignore = -1)
    {
        var fallenbricks = new HashSet<int>();
        var dropped = true;
        while (dropped)
        {
            dropped = false;
            foreach (var i in blocks.Keys)
            {
                if (i == ignore)
                    continue;

                var block = blocks[i];
                var dropper = block.Drop();
                while (dropper is not null)
                {
                    var overlap = false;
                    foreach (var (j,b) in blocks)
                    {
                        if (i == j || j == ignore)
                            continue;
                        if (b.OverlapsWith(dropper))
                        {
                            overlap = true;
                            break;
                        }

                    }

                    if (overlap)
                        break;

                    dropped = true;
                    blocks[i] = dropper;
                    fallenbricks.Add(i);
                    // See if we can drop further
                    dropper = dropper.Drop();
                }
            }
        }
        return fallenbricks.Count;
    }

    private record BlockPt(int X, int Y, int Z)
    {
        public BlockPt(int[] coords)
            : this(coords[0], coords[1], coords[2])
        {}

        /// <summary>
        /// Makes debugging so much easier
        /// </summary>
        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }
    }
    private record Block(BlockPt From, BlockPt To)
    {
        public int X0 => From.X;
        public int X1 => To.X;
        public int Y0 => From.Y;
        public int Y1 => To.Y;
        public int Z0 => From.Z;
        public int Z1 => To.Z;
        public static Block Create(string coordText)
        {
            // Assume text is of the form "1,1,8~1,1,9"
            var sp = coordText.Split('~');
            var l = sp[0].Split(',');
            var r = sp[1].Split(',');
            var p0 = new BlockPt(l.Select(int.Parse).ToArray());
            var p1 = new BlockPt(r.Select(int.Parse).ToArray());
            return new Block(p0, p1);
        }

        public bool OverlapsWith(Block other)
        {
            return this.X0 <= other.X1 && this.X1 >= other.X0
                && this.Y0 <= other.Y1 && this.Y1 >= other.Y0
                && this.Z0 <= other.Z1 && this.Z1 >= other.Z0;
        }

        public bool CanDrop => Z0 > 1 && Z1 > 1;
        public Block? Drop()
        {
            if (Z0 <= 1 || Z1 <= 1)
                //throw new Exception("Block is already on the ground");
                return null;

            var from = From with { Z = From.Z - 1 };
            var to = To with { Z = To.Z - 1 };
            return new Block(from, to);
        }

        /// <summary>
        /// Makes debugging so much easier
        /// </summary>
        public override string ToString()
        {
            return $"({X0},{Y0},{Z0})~({X1},{Y1},{Z1})";
        }
    }
}
