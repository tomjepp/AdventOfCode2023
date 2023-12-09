using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

var seeds = new List<long>();
Dictionary<string, Map> mapsBySource = new Dictionary<string, Map>();

var mapHeaderRegex = new Regex("^([a-z]+)-to-([a-z]+) map:$", RegexOptions.Compiled);

using (var reader = File.OpenText("input.txt"))
{
    string seedLine = reader.ReadLine() ?? throw new NullReferenceException();
    var seedLinePieces = seedLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    var seedStrings = seedLinePieces[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    foreach (var seedString in seedStrings)
    {
        seeds.Add(long.Parse(seedString));
    }

    while (!reader.EndOfStream)
    {
        var mapHeaderLine = reader.ReadLine();
        if (mapHeaderLine == null || string.IsNullOrWhiteSpace(mapHeaderLine))
        {
            continue;
        }

        var match = mapHeaderRegex.Match(mapHeaderLine);
        if (!match.Success)
        {
            throw new Exception("Parse error?");
        }

        string from = match.Groups[1].Value;
        string to = match.Groups[2].Value;
        var ranges = new List<Range>();

        var rangeLine = reader.ReadLine();
        while (!(rangeLine == null || string.IsNullOrWhiteSpace(rangeLine)))
        {
            var rangeLinePieces =
                rangeLine.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var range = new Range()
            {
                DestinationStart = long.Parse(rangeLinePieces[0]),
                SourceStart = long.Parse(rangeLinePieces[1]),
                Length = long.Parse(rangeLinePieces[2]),
            };
            ranges.Add(range);
            rangeLine = reader.ReadLine();
        }

        var map = new Map(from, to, ranges);
        mapsBySource.Add(from, map);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
var seedLocations = new Dictionary<long, long>();
foreach (var seed in seeds)
{
    string mapSource = "seed";
    long sourceValue = seed;

    while (mapsBySource.ContainsKey(mapSource))
    {
        var map = mapsBySource[mapSource];
        sourceValue = map.MapValue(sourceValue);
        mapSource = map.Destination;
    }

    seedLocations.Add(seed, sourceValue);
}

Console.WriteLine(seedLocations.Values.Min());

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Map(string source, string destination, List<Range> ranges)
{
    public string Source { get; } = source;
    public string Destination { get; } = destination;
    public List<Range> Ranges { get; } = ranges;

    public long MapValue(long sourceValue)
    {
        foreach (var range in Ranges)
        {
            if (range.ContainsValue(sourceValue))
            {
                return range.ConvertValue(sourceValue);
            }
        }

        return sourceValue;
    }
}

class Range
{
    public long DestinationStart { get; set; }
    public long SourceStart { get; set; }
    public long Length { get; set; }
    public long DestinationEnd => DestinationStart + Length-1;
    public long SourceEnd => SourceStart + Length-1;

    public bool ContainsValue(long sourceValue) => (sourceValue >= SourceStart && sourceValue <= SourceEnd);
    public long ConvertValue(long sourceValue) => DestinationStart + (sourceValue - SourceStart);
}
