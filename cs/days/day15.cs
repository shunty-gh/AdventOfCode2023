namespace Shunty.AoC;

// https://adventofcode.com/2023/day/15 - Lens Library

public class Day15 : AocDaySolver
{
    public int DayNumber => 15;

    public async Task Solve()
    {
        //System.Diagnostics.Debug.Assert(52 == GetHash("HASH"));
        //var input = await AocUtils.GetDayText(DayNumber, "test");
        var input = await AocUtils.GetDayText(DayNumber);
        var items = input.Split(',');

        // Part 1
        var p1 = items.Aggregate(0, (acc, s) => acc + Hash(s));
        this.ShowDayResult(1, p1);

        // Part 2
        List<Lense>[] boxes = new List<Lense>[256];
        for (var i = 0; i < 256; i++)
        {
            boxes[i] = [];
        }

        foreach (var item in items)
        {
            var isremove = !item.Contains('=');
            var label = isremove ? item[..^1] : item.Split('=')[0];
            var box = boxes[Hash(label)];
            var lid = box.FindIndex(l => l.Label == label);

            if (isremove && lid >= 0)
            {
                box.RemoveAt(lid);
            }
            else if (!isremove)
            {
                var fl = int.Parse(item.Split('=')[1]);
                if (lid >= 0)
                {
                    box[lid] = box[lid] with { FocalLength = fl };
                }
                else
                {
                    box.Add(new(label, fl));
                }
            }
        }
        var p2 = 0;
        for (var bi = 0; bi < 256; bi++)
        {
            var box = boxes[bi];
            var boxfp = 0;
            for (var si = 0; si < box.Count; si++)
            {
                boxfp += FocalPower(bi, si, box[si].FocalLength);
            }
            p2 += boxfp;
        }
        this.ShowDayResult(2, p2);
    }

    private int FocalPower(int boxIndex, int slotIndex, int focalLength) =>
        (boxIndex + 1) * (slotIndex + 1) * focalLength;

    private int Hash(string item) =>
        item.ToCharArray().Aggregate(0, (acc, c) => (acc + (int)c) * 17 % 256);

    private record Lense(string Label, int FocalLength);
}
