using System.Collections.ObjectModel;
using Spectre.Console.Rendering;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/2 - Cube Conundrum

public class Day02 : AocDaySolver
{
    public int DayNumber => 2;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);
        var games = new List<Game>();
        foreach (var s in input)
        {
            var gameparts = s.Split(':');
            var gameId = int.Parse(gameparts[0].AsSpan(5));
            var roundparts = gameparts[1].Split(';');
            var rounds = new List<GameRound>();
            foreach (var p in roundparts)
            {
                var rgbparts = p.Split(',');
                foreach (var rgb in rgbparts)
                {
                    var split = rgb.Trim().Split(' ');
                    var num = int.Parse(split[0]);
                    var r = split[1] == "red" ? num : 0;
                    var g = split[1] == "green" ? num : 0;
                    var b = split[1] == "blue" ? num : 0;
                    rounds.Add(new(r,g,b));
                }
            }
            games.Add(new(gameId, rounds));
        }

        int mred = 12, mgreen = 13, mblue = 14;
        var p1 = games
            .Where(g => g.Rounds.All(r => r.Red <= mred && r.Green <= mgreen && r.Blue <= mblue))
            .Sum(g => g.GameId);
        this.ShowDayResult(1, p1);

        var p2 = games.Sum(g =>
            g.Rounds.Max(r => r.Red)
            * g.Rounds.Max(r => r.Green)
            * g.Rounds.Max(r => r.Blue));
        this.ShowDayResult(2, p2);
    }
}

public record GameRound(int Red, int Green, int Blue);
public  record Game(int GameId, ICollection<GameRound> Rounds);
