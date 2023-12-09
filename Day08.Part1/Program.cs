using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

string instructionsLine;
var nodes = new Dictionary<string, Node>();

var regex = new Regex(@"^([A-Z]+) = \(([A-Z]+), ([A-Z]+)\)$");

using (var reader = File.OpenText("input.txt"))
{
    instructionsLine = reader.ReadLine() ?? throw new NullReferenceException();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        // parse your input here
        var match = regex.Match(line);
        var node = new Node(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
        nodes.Add(node.Name, node);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

var currentNode = nodes["AAA"];
int steps = 0;
while (currentNode.Name != "ZZZ")
{
    switch (instructionsLine[steps % instructionsLine.Length])
    {
        case 'L':
            currentNode = nodes[currentNode.Left];
            break;
        case 'R':
            currentNode = nodes[currentNode.Right];
            break;
        default:
            throw new Exception("Unknown action!");
    }

    steps++;
}
// calculate & output your result here
Console.WriteLine(steps);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Node(string name, string left, string right)
{
    public string Name { get; } = name;
    public string Left { get; } = left;
    public string Right { get; } = right;
}
