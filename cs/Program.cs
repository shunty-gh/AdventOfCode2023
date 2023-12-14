global using Shunty.AoC;
using System.Diagnostics;
using Spectre.Console;

/// An over engineered shell to run one or more solution classes
/// for the [Advent Of Code 2023](https://adventofcode.com/2023) puzzles.

/// The solution classes just need to implement the `AocDaySolver` interface
/// and for the purposes of this project are saved in the `./days`
/// directory.
/// The timing of each solution is a bit academic as the AnsiConsole stuff
/// with spinners and colouring etc takes longer than normal simple stdout
/// type writing. But who cares.

// Still not quite sure if 'top-level statments' are all that great an
// idea for most projects but we'll go with it for now.

Console.WriteLine();
PrintTitle("Advent Of Code 2023 (C#)");
Console.WriteLine();

var timer = Stopwatch.StartNew();
try
{
    var daysRequested = GetDaysRequested(args);
    var runAll = daysRequested.Count == 0;
    var daysToRun = LoadSolutionTypes()
        .OrderBy(x => x.DayNumber)
        .Where(x => runAll || daysRequested.Contains(x.DayNumber));

    await RunSolutions(daysToRun, timer);
}
finally
{
    timer.Stop();
    PrintFooter(timer.ElapsedMilliseconds);
}

/// <summary>
/// Register all classes that support the `AoCDaySolver`
/// interface.
/// </summay>
static IReadOnlyCollection<AocDaySolver> LoadSolutionTypes()
{
    return new List<AocDaySolver>
    {
        new Day01(), new Day02(), new Day03(), new Day04(), new Day05(),
        new Day06(), new Day07(), new Day08(), new Day09(), new Day10(),
        new Day11(), new Day12(), new Day13(), new Day14(), //new Day15(),
        //new Day16(), new Day17(), new Day18(), new Day19(), new Day20(),
        //new Day21(), new Day22(), new Day23(), new Day24(), new Day25(),
    };
}

/// <summary>
/// Parse the command line for any day numbers required
/// </summary>
static IReadOnlyCollection<int> GetDaysRequested(string[] args)
{
    var today = DateTime.Today;
    var curentDay = (today.Year == 2023 && today.Month == 12 && today.Day <= 25) ? today.Day : 0;
    var runAll = args.Any(a => a == "-a" || a == "--all");
    if (runAll)
        return [];

    List<int> daysRequested = args  // Get any day numbers off the command line
        .Select(a => int.TryParse(a, out var ia) ? ia : 0)
        .Where(i => i > 0 && i <= 25)
        .OrderBy(x => x)
        .Distinct()
        .ToList();
    if (daysRequested.Count == 0 && curentDay > 0)
    {
        daysRequested.Add(curentDay);
    }
    return daysRequested;
}

/// <summary>
/// Run each day solution and make it look good
/// </summary>
static async Task RunSolutions(IEnumerable<AocDaySolver> daysToRun, Stopwatch timer)
{
    await AnsiConsole.Status()
        .StartAsync("Running...", async ctx =>
        {

            var flipflop = false;
            ctx.Spinner(Spinner.Known.Christmas);

            foreach (var daySolution in daysToRun)
            {
                // A bit of unnecessary fluff
                ctx.Status($"Day {daySolution.DayNumber}");
                ctx.Spinner(flipflop ? Spinner.Known.Star : Spinner.Known.Christmas);
                flipflop = !flipflop;
                //await Task.Delay(2000);  // Optional - So we get time to see the pretty spinners :-)

                // Run it and time it
                AnsiConsole.MarkupLine($"[bold]Day {daySolution.DayNumber}[/]");
                var start = timer.ElapsedMilliseconds;
                await daySolution.Solve();
                AnsiConsole.MarkupLine($"  [blue]Day {daySolution.DayNumber} completed in [yellow]{timer.ElapsedMilliseconds - start}[/]ms[/]");
            }
        });
}

/// <summary>
/// Output the given title text to the console in random ANSI
/// colouring (if the shell supports it)
/// </summary>
static void PrintTitle(string title)
{
    var colours = new string[] { "red", "orange1", "yellow", "green", "blue", "purple", "violet" };
    var rnd = new Random();
    foreach(var c in $" * * * {title} * * * ")
    {
        var colour = colours[rnd.Next(colours.Length)];
        AnsiConsole.Markup($"[bold {colour}]{c}[/]");
    }
    AnsiConsole.WriteLine();
}

static void PrintFooter(long elapsedMilliseconds)
{
    Console.WriteLine();
    AnsiConsole.MarkupLine($"[blue]All completed in [yellow]{elapsedMilliseconds}[/]ms[/]");
    Console.WriteLine();
}
