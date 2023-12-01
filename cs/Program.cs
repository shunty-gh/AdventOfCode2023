global using Spectre.Console;
global using Shunty.AoC;
using System.Diagnostics;
using System.Reflection;


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
    try
    {
        var today = DateTime.Today;
        var curentDay = (today.Year == 2023 && today.Month == 12 && today.Day <= 25) ? today.Day : 0;
        var daysToRun = args  // Get any day numbers off the command line
            .Select(a => int.TryParse(a, out var ia) ? ia : 0)
            .Where(i => i > 0 && i <= 25)
            .OrderBy(x => x)
            .Distinct()
            .ToList();
        if (daysToRun.Count == 0 && curentDay > 0)
        {
            daysToRun.Add(curentDay);
        }
        var runAll = daysToRun.Count == 0 || args.Any(a => a == "-a" || a == "--all");
        var availableDays = LoadSolutionTypes()
            .OrderBy(x => x.day)
            .Where(x => runAll || daysToRun.Contains(x.day));

        await AnsiConsole.Status()
            .StartAsync("Running...", async ctx =>
            {
                var flipflop = false;
                ctx.Spinner(Spinner.Known.Christmas);

                foreach (var (dayno,daytype) in availableDays)
                {
                    // A bit of unnecessary fluff
                    ctx.Status($"Day {dayno}");
                    ctx.Spinner(flipflop ? Spinner.Known.Star : Spinner.Known.Christmas);
                    flipflop = !flipflop;
                    //await Task.Delay(2000);  // Optional - So we get time to see the pretty spinners :-)

                    // Create the class, run it and time it
                    if (Activator.CreateInstance(daytype) is AocDaySolver soln)
                    {
                        AnsiConsole.MarkupLine($"[bold]Day {dayno}[/]");
                        var start = timer.ElapsedMilliseconds;
                        await soln.Solve();
                        AnsiConsole.MarkupLine($"  [blue]Day {dayno} completed in [yellow]{timer.ElapsedMilliseconds - start}[/]ms[/]");
                    }
                }
            });
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex);
    }
}
finally
{
    timer.Stop();
    AnsiConsole.MarkupLine($"[blue]All completed in [yellow]{timer.ElapsedMilliseconds}[/]ms[/]");
    Console.WriteLine();
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

/// <summary>
/// Get all types in this assembly that support the `AoCDaySolver`
/// interface (except for the interface itself).
/// Return a list rather than a dictionary so we can add more than one way
/// of solving a day should we really want to.
/// </summay>
static IList<(int day, Type daySolver)> LoadSolutionTypes() 
{
    var result = new List<(int, Type)>();
    var dstype = typeof(AocDaySolver);
    var solutiontypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(type => dstype.IsAssignableFrom(type)
            && type != dstype
            && !type.IsAbstract);

    foreach (Type type in solutiontypes)
    {
        // Run/Invoke the static method (of the AoCDaySolver interface) to get the day
        // number for which the type/class is made for
        var dnmeth = type.GetMethod(nameof(AocDaySolver.DayNumber), BindingFlags.Public | BindingFlags.Static);
        var dn = (int)(dnmeth?.Invoke(null, null) ?? 0);
        // ...or...
        // var dn = (int)(type.InvokeMember(
        //     nameof(AocDaySolver.DayNumber),
        //     BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, null) ?? 0);

        result.Add((dn, type));
    }

    return result;
}
