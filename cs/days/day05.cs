namespace Shunty.AoC;

// https://adventofcode.com/2023/day/5

public class Day05 : AocDaySolver
{
    public int DayNumber => 5;

    public async Task Solve()
    {
        var maps = new Dictionary<string, List<AlmanacMapItem>>();
        var input = await AocUtils.GetDayLines(DayNumber);
        var seeds = input[0].Split(':')[1].Trim().Split(' ').Select(s => Int64.Parse(s));
        var currentMap = "";
        foreach (var line in input.Skip(2))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (line.Trim().EndsWith("map:"))
            {
                currentMap = line.Substring(0, line.Length - 5);
                maps.Add(currentMap, new());
            }
            else
            {
                var mapsplit = line.Split(' ').Select(s => Int64.Parse(s)).ToList();
                maps[currentMap].Add(new(mapsplit[0], mapsplit[0] + mapsplit[2] - 1, mapsplit[1], mapsplit[1] + mapsplit[2] - 1, mapsplit[2]));
            }
        }

        var locations = new Dictionary<Int64, Int64>();
        foreach (var seed in seeds)
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
            var loc = GetDestination(humid, maps["humidity-to-location"]);
            locations.Add(seed, loc);
        }

        var p1 = locations.Values.Min();
        var p2 = 0;
        this.ShowDayResult(1, p1);
        this.ShowDayResult(2, p2);
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