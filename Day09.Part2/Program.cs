using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var sequences = new List<Sequence>();

using (var reader = File.OpenText("input.txt"))
{
    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        var sequenceValueStrings = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var sequenceValues = new List<long>();
        foreach (var sequenceValueString in sequenceValueStrings)
        {
            sequenceValues.Add(long.Parse(sequenceValueString));
        }

        var sequence = new Sequence(sequenceValues);
        sequences.Add(sequence);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
long sum = 0;
foreach (var sequence in sequences)
{
    var nextValue = sequence.PreviousValue();
    Console.WriteLine(nextValue);
    sum += nextValue;
}
Console.WriteLine();
Console.WriteLine(sum);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Sequence
{
    private List<long> Values { get; }
    private List<long> Deltas { get; }
    private readonly Sequence? _childSequence;

    public Sequence(List<long> values)
    {
        Values = values;
        Deltas = new List<long>();

        for (var i = 0; i < values.Count - 1; i++)
        {
            var delta = values[i + 1] - values[i];
            Deltas.Add(delta);
        }

        _childSequence = Deltas.TrueForAll(x => x == 0) ? null : new Sequence(Deltas);
    }

    public long NextValue()
    {
        var lastValue = Values.Last();
        var nextDelta = _childSequence?.NextValue() ?? 0;
        var newValue = lastValue + nextDelta;
        Values.Add(newValue);
        return newValue;
    }

    public long PreviousValue()
    {
        var firstValue = Values.First();
        var nextDelta = _childSequence?.PreviousValue() ?? 0;
        var newValue = firstValue - nextDelta;
        Values.Insert(0, newValue);
        return newValue;
    }
}
