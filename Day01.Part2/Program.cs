﻿using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var calibrationValues = new List<List<int>>();

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

        var valuesForLine = new List<int>();

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (char.IsDigit(c))
            {
                valuesForLine.Add(int.Parse(c.ToString()));
            }
            else if (line.Substring(i).StartsWith("one"))
            {
                valuesForLine.Add(1);
            }
            else if (line.Substring(i).StartsWith("two"))
            {
                valuesForLine.Add(2);
            }
            else if (line.Substring(i).StartsWith("three"))
            {
                valuesForLine.Add(3);
            }
            else if (line.Substring(i).StartsWith("four"))
            {
                valuesForLine.Add(4);
            }
            else if (line.Substring(i).StartsWith("five"))
            {
                valuesForLine.Add(5);
            }
            else if (line.Substring(i).StartsWith("six"))
            {
                valuesForLine.Add(6);
            }
            else if (line.Substring(i).StartsWith("seven"))
            {
                valuesForLine.Add(7);
            }
            else if (line.Substring(i).StartsWith("eight"))
            {
                valuesForLine.Add(8);
            }
            else if (line.Substring(i).StartsWith("nine"))
            {
                valuesForLine.Add(9);
            }
        }

        calibrationValues.Add(valuesForLine);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

var total = calibrationValues.Sum(x => (x.First() * 10) + x.Last());
Console.WriteLine(total);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");
