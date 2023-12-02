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
        string line = reader.ReadLine();

        if (String.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        var match = gameRegex.Match(line);
        var gameId = int.Parse(match.Groups[1].Value);
        var items = match.Groups[2].Captures.Count;

        var game = new Game(gameId);
        var ballCollection = new BallCollection();
        for (int i = 0; i < items; i++)
        {
            int numberOfBalls = int.Parse(match.Groups[2].Captures[i].Value);
            string ballColour = match.Groups[3].Captures[i].Value;
            string endChar = i < match.Groups[4].Captures.Count ? match.Groups[4].Captures[i].Value : "";

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
long sum = games.Sum(x => x.Power);
Console.WriteLine(sum);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

class Game
{
    public int ID { get; set; }
    public List<BallCollection> BallCollections { get; }

    public bool IsPossible
    {
        get
        {
            return BallCollections.All(x => x.IsPossible);
        }
    }

    public int RequiredRed
    {
        get
        {
            return BallCollections.Max(x => x.Red);
        }
    }

    public int RequiredGreen
    {
        get
        {
            return BallCollections.Max(x => x.Green);
        }
    }

    public int RequiredBlue
    {
        get
        {
            return BallCollections.Max(x => x.Blue);
        }
    }

    public int Power
    {
        get
        {
            return RequiredRed * RequiredGreen * RequiredBlue;
        }
    }

    public Game(int id)
    {
        ID = id;
        BallCollections = new List<BallCollection>();
    }
}

class BallCollection
{
    public int Red { get; set; }
    public int Green { get; set; }
    public int Blue { get; set; }

    public bool IsPossible
    {
        get
        {
            return (Red <= 12 && Green <= 13 && Blue <= 14);
        }
    }

    public BallCollection()
    {
        Blue = 0;
        Red = 0;
        Green = 0;
    }
}
