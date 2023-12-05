namespace Shunty.AoC;

// https://adventofcode.com/2023/day/5 - If You Give A Seed A Fertilizer

/// The *very* slow, brute force solution. About 20 minutes on my machine.
/// But it does, at least, give the right answer.
/// There is, obviously, a way faster solution somewhere but I just haven't
/// quite discovered it yet.


public class Day05 : AocDaySolver
{
    public int DayNumber => 5;

    public async Task Solve()
    {
        var input = await AocUtils.GetDayLines(DayNumber);

        var seeds = input[0].Split(':')[1].Trim().Split(' ').Select(s => Int64.Parse(s)).ToList();
        var maps = new Dictionary<string, List<AlmanacMapItem>>();
        var currentMap = "";
        foreach (var line in input.Skip(2))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line.Trim().EndsWith("map:"))
            {
                currentMap = line[..^5];
                maps.Add(currentMap, []);
            }
            else
            {
                var mapsplit = line.Split(' ').Select(s => Int64.Parse(s)).ToList();
                maps[currentMap].Add(
                    new(mapsplit[0],
                    mapsplit[0] + mapsplit[2] - 1,
                    mapsplit[1],
                    mapsplit[1] + mapsplit[2] - 1,
                    mapsplit[2]));
            }
        }

        // Part 1
        Int64 p1 = -1;
        foreach (var seed in seeds)
        {
            var loc = GetLocationForSeed(seed, maps);
            if (p1 < 0 || p1 > loc)
                p1 = loc;
        }
        this.ShowDayResult(1, p1);

        // Part 2
        Int64 p2 = -1;
        int seedid = 0;
        var locker = new object();
        var tasks = new List<Task>();
        while (seedid < seeds.Count)
        {
            Int64 seedbase = seeds[seedid];
            Int64 seedrange = seeds[seedid + 1];

            var t = Task.Factory.StartNew(() => {
                var lowest = GetLocationForSeedRange(seedbase, seedrange, maps.AsReadOnly());
                lock(locker)
                {
                    if (p2 < 0 || p2 > lowest)
                        p2 = lowest;
                }
            });
            tasks.Add(t);

            await Task.WhenAll(tasks);
            seedid += 2;
        }
        this.ShowDayResult(2, p2);
    }

    private Int64 GetLocationForSeedRange(Int64 seedBase, Int64 seedRange, IReadOnlyDictionary<string, List<AlmanacMapItem>> maps)
    {
        Int64 result = -1;

        for (var ds = 0; ds < seedRange; ds++)
        {
            var seed = seedBase + ds;
            var loc = GetLocationForSeed(seed, maps);
            if (result == -1 || loc < result)
                result = loc;
        }
        return result;
    }

    private Int64 GetLocationForSeed(Int64 seed, IReadOnlyDictionary<string, List<AlmanacMapItem>> maps)
    {
        // Seed to soil
        var soil = GetDestination(seed, maps["seed-to-soil"]);
        // Soil to fertilizer
        var fert = GetDestination(soil, maps["soil-to-fertilizer"]);
        // Fertilizer to water
        var water = GetDestination(fert, maps["fertilizer-to-water"]);
        // Water to light
        var light = GetDestination(water, maps["water-to-light"]);
        // Light to temperature
        var temp = GetDestination(light, maps["light-to-temperature"]);
        // Temperature to humidity
        var humid = GetDestination(temp, maps["temperature-to-humidity"]);
        // Humidity to location
        return GetDestination(humid, maps["humidity-to-location"]);
    }

    private Int64 GetDestination(Int64 source, List<AlmanacMapItem> map)
    {
        foreach (var mapping in map)
        {
            if (source >= mapping.SourceMin && source <= mapping.SourceMax)
            {
                return mapping.DestMin + (source - mapping.SourceMin);
            }
        }
        // if not found, return same as source
        return source;
    }
}

public record AlmanacMapItem(Int64 DestMin, Int64 DestMax, Int64 SourceMin, Int64 SourceMax, Int64 Range);