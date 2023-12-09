using System.Diagnostics;
using System.Net.Sockets;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

string instructionsLine;
var nodes = new Dictionary<string, Node>();

var regex = new Regex(@"^([0-9A-Z]+) = \(([0-9A-Z]+), ([0-9A-Z]+)\)$");

using (var reader = File.OpenText("input.txt"))
{
    instructionsLine = reader.ReadLine();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        // parse your input here
        var match = regex.Match(line);
        var node = new Node()
        {
            Name = match.Groups[1].Value,
            Left = match.Groups[2].Value,
            Right = match.Groups[3].Value,
        };
        nodes.Add(node.Name, node);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

var paths = new List<Path>();
foreach (var node in nodes.Values)
{
    if (node.Name.EndsWith("A"))
    {
        var path = new Path()
        {
            StartingNode = node,
        };
        paths.Add(path);
    }
}

// find first wins
foreach (var path in paths)
{
    var currentNode = path.StartingNode;
    int steps = 0;
    while (steps == 0 || !currentNode.Name.EndsWith("Z"))
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

    path.EndingNode = currentNode;
    path.FirstWinStepsCount = steps;
}

// find second wins
foreach (var path in paths)
{
    var currentNode = path.EndingNode;
    int steps = path.FirstWinStepsCount;
    while (steps == path.FirstWinStepsCount || !currentNode.Name.EndsWith("Z"))
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

    path.SecondWinStepsCount = steps;

    if (path.EndingNode != currentNode)
    {
        throw new Exception("violated my assumption that we'll always loop back to the same ending node");
    }

    if (path.SecondWinStepsCount != path.FirstWinStepsCount * 2)
    {
        throw new Exception("violated my assumption that loops are always the same length");
    }
}

// calculate & output your result here
long result = LowestCommonMultipleGroup(paths.Select(x => (long)x.FirstWinStepsCount));
Console.WriteLine(result);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

long LowestCommonMultipleGroup(IEnumerable<long> numbers)
{
    return numbers.Aggregate(LowestCommonMultiple);
}
long LowestCommonMultiple(long a, long b)
{
    return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
}
long GreatestCommonDivisor(long a, long b)
{
    return b == 0 ? a : GreatestCommonDivisor(b, a % b);
}

class Node
{
    public string Name { get; set; }
    public string Left { get; set; }
    public string Right { get; set; }
}

class Path
{
    public Node StartingNode { get; set; }
    public Node EndingNode { get; set; }
    public int FirstWinStepsCount { get; set; }
    public int SecondWinStepsCount { get; set; }
}
