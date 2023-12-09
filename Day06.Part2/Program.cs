using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

Race race = null;
using (var reader = File.OpenText("input.txt"))
{

    while (!reader.EndOfStream)
    {
        var timeLine = reader.ReadLine();
        var timeLinePieces = timeLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var durationString = timeLinePieces[1].Replace(" ", "");
        var distanceLine = reader.ReadLine();
        var distanceLinePieces = distanceLine.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var distanceString = distanceLinePieces[1].Replace(" ", "");
        race = new Race()
        {
            DurationMs = long.Parse(durationString),
            RecordDistance = long.Parse(distanceString)
        };
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

long product = 1;
// calculate & output your result here
race.CalculateDistances();
Console.WriteLine(race.WinningDistancesCount);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Race
{
    public long DurationMs { get; set; }
    public long RecordDistance { get; set; }

    public long WinningDistancesCount { get; set; }

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
                WinningDistancesCount++;
            }
        }
    }
}
