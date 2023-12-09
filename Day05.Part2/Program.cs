using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

var seedRanges = new List<Range>();
Dictionary<string, Map> mapsByDestination = new Dictionary<string, Map>();

var mapHeaderRegex = new Regex("^([a-z]+)-to-([a-z]+) map:$", RegexOptions.Compiled);

using (var reader = File.OpenText("input.txt"))
{
    string seedLine = reader.ReadLine() ?? throw new NullReferenceException();
    var seedLinePieces = seedLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    var seedStrings = seedLinePieces[1].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < seedStrings.Length; i += 2)
    {
        var start = long.Parse(seedStrings[i]);
        var length = long.Parse(seedStrings[i+1]);
        var range = new Range()
        {
            SourceStart = start,
            Length = length,
        };
        seedRanges.Add(range);
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
        mapsByDestination.Add(to, map);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

bool gotMatch = false;
// calculate & output your result here
long location = 0;
while (!gotMatch)
{
    location++;
    string mapDestination = "location";
    long destinationValue = location;

    while (mapsByDestination.ContainsKey(mapDestination))
    {
        var map = mapsByDestination[mapDestination];
        destinationValue = map.MapDestinationValue(destinationValue);
        mapDestination = map.Source;
    }

    foreach (var range in seedRanges)
    {
        if (range.ContainsValue(destinationValue))
        {
            gotMatch = true;
            break;
        }
    }
}

Console.WriteLine(location);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Map(string source, string destination, List<Range> ranges)
{
    public string Source { get; } = source;
    public string Destination { get; } = destination;
    private List<Range> Ranges { get; } = ranges;

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
    public long MapDestinationValue(long destinationValue)
    {
        foreach (var range in Ranges)
        {
            if (range.ContainsDestinationValue(destinationValue))
            {
                return range.ConvertDestinationValue(destinationValue);
            }
        }

        return destinationValue;
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
    public bool ContainsDestinationValue(long destinationValue) => (destinationValue >= DestinationStart && destinationValue <= DestinationEnd);
    public long ConvertValue(long sourceValue) => DestinationStart + (sourceValue - SourceStart);
    public long ConvertDestinationValue(long destinationValue) => SourceStart + (destinationValue - DestinationStart);
}
