namespace Shunty.AoC;

// https://adventofcode.com/2023/day/3 - Gear Ratios

public class Day03 : AocDaySolver
{
    public int DayNumber => 3;

    public async Task Solve()
    {
        var input = (await AocUtils.GetDayLines(DayNumber)).ToList();

        var hhh = input.Count;
        var www = input[0].Length;
        var symbols = new Dictionary<Pt, (char Symbol, ICollection<int> NextTo)>();
        var numbers = new Dictionary<Pt, int>();
        for (var y = 0; y < hhh; y++)
        {
            var row = input[y];
            for (var x = 0; x < www; x++)
            {
                var cell = row[x];
                if (Char.IsDigit(cell))
                {
                    var n = "" + cell;
                    // get number
                    var x2 = x;
                    while (x2++ < www-1)
                    {
                        if (!Char.IsDigit(row[x2]))
                            break;
                        n += row[x2];
                    }
                    numbers.Add(new(x,y), int.Parse(n));
                    x += n.Length - 1;
                }
                else if (cell != '.') // add a symbol
                {
                    symbols.Add(new(x,y), (cell, new List<int>()));
                }
            }
        }

        var p1 = 0;
        foreach (var ((x1,y),num) in numbers)
        {
            // Make a list of all points surrounding this number
            var x2 = x1 + num.ToString().Length - 1;
            List<Pt> nearby = [ new(x1-1,y), new(x2+1,y) ]; // before the beginning and after the end
            for (var x = x1-1; x <= x2+1; x++)
            {
                nearby.Add(new(x,y-1)); // points above the number
                nearby.Add(new(x,y+1)); // points below the number
            }

            // See if the surrounding points contain a symbol
            var p1_add = false;
            foreach (var pp in nearby)
            {
                if (symbols.ContainsKey(pp))
                {
                    p1_add = true;

                    // Is it next to a star? If so, add to the list of values next to that star
                    if (symbols[pp].Symbol == '*')
                    {
                        symbols[pp].NextTo.Add(num);
                    }
                }
            }
            if (p1_add)
                p1 += num;
        }
        this.ShowDayResult(1, p1);

        var p2 = symbols
            .Where(kvp => kvp.Value.NextTo.Count == 2)  // all stars next to exactly 2 numbers
            .Select(kvp => kvp.Value.NextTo.First() * kvp.Value.NextTo.Last()) // get the 'gear ratio'
            .Sum();
        this.ShowDayResult(2, p2);
    }
}

public record Pt(int X, int Y);