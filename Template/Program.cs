using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

using (var reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null)
        {
            continue;
        }

        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        // parse your input here
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
Console.WriteLine("output");

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");
