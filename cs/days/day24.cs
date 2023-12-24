//#define TEST
namespace Shunty.AoC;

// https://adventofcode.com/2023/day/24

public class Day24 : AocDaySolver
{
    public int DayNumber => 24;

#if TEST
    private long P1BoundLo = 7;
    private long P1BoundHi = 27;
#else
    private long P1BoundLo = 200000000000000;
    private long P1BoundHi = 400000000000000;
#endif

    public async Task Solve()
    {
#if TEST
        var input = await AocUtils.GetDayLines(DayNumber, "test");
#else
        var input = await AocUtils.GetDayLines(DayNumber);
#endif
        this.ShowDayResult(1, Part1(input));
        this.ShowDayResult(2, 0);
    }

    private record Pt3(double X, double Y, double Z);
    private record Pt2(double X, double Y, double Z);
    private record HailStone(Pt3 Loc, Pt2 Vel)
    {
        public double X => Loc.X;
        public double Y => Loc.Y;
        public double Z => Loc.Z;
    }

    private int Part1(List<string> input)
    {
        var stones = new List<HailStone>();
        foreach (var line in input)
        {
            var sp = line.Split(" @ ");
            var pts = sp[0].Trim().Split(", ").Select(double.Parse).ToArray();
            var vel = sp[1].Trim().Split(", ").Select(double.Parse).ToArray();
            stones.Add(new HailStone(new Pt3(pts[0],pts[1],pts[2]), new Pt2(vel[0],vel[1],vel[2])));
        }

        var result = 0;
        for (var i = 0; i < stones.Count; i++)
        {
            var stone = stones[i];
            for (var j = i; j < stones.Count; j++)
            {
                var check = stones[j];
                if (Intersects(stone, check))
                {
                    result += 1;
                }
            }
        }
        return result;
    }

    private bool Intersects(HailStone a, HailStone b)
    {
        double aa = a.Vel.Y / a.Vel.X;
        double cc = a.Y - (aa * a.X);
        double bb = b.Vel.Y / b.Vel.X;
        double dd = b.Y - (bb * b.X);

        if (aa-bb == 0)
            return false;

        double xi = (dd - cc) / (aa - bb);
        double yi = (aa * xi) + cc;
        // Out of bounds?
        if (xi < P1BoundLo || xi > P1BoundHi || yi < P1BoundLo || yi > P1BoundHi)
            return false;
        // In the future?
        // xi = x0 + vx*t find t
        // (xi - x0) / vx = t
        // t must be > 0
        if (((xi - a.X) / a.Vel.X) >= 1 && ((xi - b.X) / b.Vel.X) >= 1)
            return true;

        return false;
    }
}
