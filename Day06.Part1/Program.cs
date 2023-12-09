using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var races = new List<Race>();
using (var reader = File.OpenText("input.txt"))
{

    while (!reader.EndOfStream)
    {
        var timeLine = reader.ReadLine();
        var timeLinePieces = timeLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var durationStrings = timeLinePieces[1]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var distanceLine = reader.ReadLine();
        var distanceLinePieces = distanceLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var distanceStrings = distanceLinePieces[1]
            .Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (durationStrings.Length != distanceStrings.Length)
        {
            throw new Exception("Time & distance mismatch?");
        }

        for (int i = 0; i < durationStrings.Length; i++)
        {
            var race = new Race()
            {
                DurationMs = int.Parse(durationStrings[i]),
                RecordDistance = int.Parse(distanceStrings[i]),
            };
            races.Add(race);
        }
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

long product = 1;
// calculate & output your result here
foreach (var race in races)
{
    race.CalculateDistances();
    product *= race.WinningDistances.Count;
}
Console.WriteLine(product);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Race
{
    public int DurationMs { get; set; }
    public int RecordDistance { get; set; }

    public Dictionary<int, int> WinningDistances { get; set; } = new Dictionary<int, int>();

    public void CalculateDistances()
    {
        var beatenRecord = false;
        for (int i = 1; i < DurationMs; i++)
        {
            var distance = (DurationMs - i) * i;
            if (distance <= RecordDistance)
            {
                if (beatenRecord)
                {
                    break;
                }

                continue;
            }
            else
            {
                beatenRecord = true;
                WinningDistances.Add(i, distance);
            }
        }
    }
}
