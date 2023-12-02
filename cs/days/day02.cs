using System.Collections.ObjectModel;

namespace Shunty.AoC;

// https://adventofcode.com/2023/day/2

public class Day02 : AocDaySolver
{
    public int DayNumber => 2;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);
        var p1 = 0;
        var games = new List<Game>();
        foreach (var s in input)
        {
            var gameparts = s.Split(':');
            var gameId = int.Parse(gameparts[0].AsSpan(5));
            var roundparts = gameparts[1].Split(';');
            var rounds = new List<GameRound>();
            foreach (var p in roundparts)
            {
                int r = 0, g = 0, b = 0;
                var rgbparts = p.Split(',');
                foreach (var rgb in rgbparts)
                {
                    var split = rgb.Trim().Split(' ');
                    var num = int.Parse(split[0]);
                    switch (split[1].Trim())
                    {
                        case "red":
                            r = num;
                            break;
                        case "green":
                            g = num;
                            break;
                        case "blue":
                            b = num;
                            break;
                    }
                    rounds.Add(new(r,g,b));
                }
            }
            var game = new Game { GameId = gameId, Rounds = rounds };
            games.Add(game);
        }

        int mred = 12, mgreen = 13, mblue = 14;
        foreach (var game in games)
        {
            if (game.Rounds.All(r => r.Red <= mred && r.Green <= mgreen && r.Blue <= mblue))
            {
                p1 += game.GameId;
            }
        }
        this.ShowDayResult(1, p1);
    }
}

public struct GameRound
{
    public GameRound(int r, int g, int b)
    {
        Red = r;
        Green = g;
        Blue = b;
    }
    public int Red { get; init; }
    public int Green { get; init; }
    public int Blue { get; init; }
}
public record struct Game
{
    public int GameId { get; set; }
    public List<GameRound> Rounds { get; set; }
}
