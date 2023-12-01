using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var calibrationValues = new List<List<int>>();

using (var reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        string line = reader.ReadLine();
        if (String.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        var valuesForLine = new List<int>();

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (char.IsDigit(c))
            {
                valuesForLine.Add(int.Parse(c.ToString()));
            }
        }

        calibrationValues.Add(valuesForLine);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

long total = 0;
foreach (var valuesForLine in calibrationValues)
{
    int value = (valuesForLine.First() * 10) + valuesForLine.Last();
    total += value;
}
Console.WriteLine(total);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");
