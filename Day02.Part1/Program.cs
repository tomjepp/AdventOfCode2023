using System.Diagnostics;
using System.Text.RegularExpressions;

var stopwatch = new Stopwatch();

stopwatch.Start();

var gameRegex = new Regex(@"^Game (\d+):(?: (\d+) (blue|red|green)(,|;)?)+$", RegexOptions.Compiled);

var games = new List<Game>();

using (var reader = File.OpenText("input.txt"))
{
    // parse your input here
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

        var match = gameRegex.Match(line);
        var gameId = int.Parse(match.Groups[1].Value);
        var items = match.Groups[2].Captures.Count;

        var game = new Game(gameId);
        var ballCollection = new BallCollection();
        for (var i = 0; i < items; i++)
        {
            var numberOfBalls = int.Parse(match.Groups[2].Captures[i].Value);
            var ballColour = match.Groups[3].Captures[i].Value;
            var endChar = i < match.Groups[4].Captures.Count ? match.Groups[4].Captures[i].Value : "";

            switch (ballColour.ToLowerInvariant())
            {
                case "red":
                    ballCollection.Red = numberOfBalls;
                    break;
                case "green":
                    ballCollection.Green = numberOfBalls;
                    break;
                case "blue":
                    ballCollection.Blue = numberOfBalls;
                    break;
            }

            switch (endChar)
            {
                case ",":
                    // do nothing;
                    break;
                case ";": // end of a specific collection
                case "": // end of the line!
                    game.BallCollections.Add(ballCollection);
                    ballCollection = new BallCollection();
                    break;
            }
        }

        games.Add(game);
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
long sum = games.Where(x => x.IsPossible).Sum(x => x.Id);
Console.WriteLine(sum);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

internal class Game(int id)
{
    public int Id { get; set; } = id;
    public List<BallCollection> BallCollections { get; } = new();
    public bool IsPossible => BallCollections.All(x => x.IsPossible);
}

internal class BallCollection
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }
    public bool IsPossible => (Red <= 12 && Green <= 13 && Blue <= 14);
}
