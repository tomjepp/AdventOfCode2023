using System.Diagnostics;
using Helpers;

var stopwatch = new Stopwatch();

stopwatch.Start();

var grid = new Dictionary<Coordinate, Pipe>();
Pipe? startingPipe = null;

using (var reader = File.OpenText("input.txt"))
{
    int row = 0;

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        for (var column = 0; column < line.Length; column++)
        {
            var type = line[column] switch
            {
                '.' => PipeType.None,
                '|' => PipeType.Vertical,
                '-' => PipeType.Horizontal,
                'L' => PipeType.L,
                'J' => PipeType.J,
                '7' => PipeType.Seven,
                'F' => PipeType.F,
                'S' => PipeType.Start,
                _ => PipeType.None
            };

            if (type == PipeType.None)
            {
                continue;
            }

            var location = new Coordinate()
            {
                X = column + 1,
                Y = row + 1,
            };

            var pipe = new Pipe()
            {
                Location = location,
                Type = type,
            };
            grid.Add(pipe.Location, pipe);

            if (type == PipeType.Start)
            {
                startingPipe = pipe;
            }
        }

        row++;
    }
}

foreach (var pair in grid)
{
    pair.Value.Connect(grid);
}
startingPipe.SetTypeFromConnections();

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

// calculate & output your result here
var lastPipe = startingPipe;
var currentPipe = startingPipe.FindNextPipe(null);
int steps = 1;
while (currentPipe != startingPipe)
{
    var nextPipe = currentPipe.FindNextPipe(lastPipe);
    lastPipe = currentPipe;
    currentPipe = nextPipe;
    steps++;
}

int furthestDistance = steps / 2;
Console.WriteLine(furthestDistance);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

internal enum PipeType
{
    None,
    Vertical, // North/South - Up/Down
    Horizontal, // East/West - Left/Right
    L, // turn, north/east - up/right
    J, // turn, north/west - up/left
    Seven, // turn, south/west - down/left
    F, // turn, south/east - down/right
    Start, // no pipe should be this type permanently
}

internal class Pipe
{
    public Coordinate Location { get; set; }

    public PipeType Type { get; set; }

    public Pipe? North { get; set; }
    public Pipe? South { get; set; }
    public Pipe? East { get; set; }
    public Pipe? West { get; set; }

    public void Connect(Dictionary<Coordinate, Pipe> grid)
    {
        Pipe? north, south, east, west;
        grid.TryGetValue(Location.North(), out north);
        grid.TryGetValue(Location.South(), out south);
        grid.TryGetValue(Location.East(), out east);
        grid.TryGetValue(Location.West(), out west);

        if (north != null && CanConnectNorth() && north.CanConnectSouth())
        {
            North = north;
        }

        if (south != null && CanConnectSouth() && south.CanConnectNorth())
        {
            South = south;
        }

        if (east != null && CanConnectEast() && east.CanConnectWest())
        {
            East = east;
        }

        if (west != null && CanConnectWest() && west.CanConnectEast())
        {
            West = west;
        }
    }

    public bool CanConnectNorth() => Type == PipeType.Vertical || Type == PipeType.L || Type == PipeType.J || Type == PipeType.Start;
    public bool CanConnectSouth() => Type == PipeType.Vertical || Type == PipeType.Seven || Type == PipeType.F || Type == PipeType.Start;
    public bool CanConnectEast() => Type == PipeType.Horizontal || Type == PipeType.L || Type == PipeType.F || Type == PipeType.Start;
    public bool CanConnectWest() => Type == PipeType.Horizontal || Type == PipeType.J || Type == PipeType.Seven || Type == PipeType.Start;

    public void SetTypeFromConnections()
    {
        if (North != null && South != null)
        {
            Type = PipeType.Vertical;
        }
        else if (East != null && West != null)
        {
            Type = PipeType.Horizontal;
        }
        else if (North != null && East != null)
        {
            Type = PipeType.L;
        }
        else if (North != null && West != null)
        {
            Type = PipeType.J;
        }
        else if (South != null && West != null)
        {
            Type = PipeType.Seven;
        }
        else if (South != null && East != null)
        {
            Type = PipeType.F;
        }
        else
        {
            throw new Exception("Impossible connection type");
        }
    }

    public Pipe FindNextPipe(Pipe? cameFrom)
    {
        if (North != null && North != cameFrom)
        {
            return North;
        }

        if (South != null && South != cameFrom)
        {
            return South;
        }

        if (East != null && East != cameFrom)
        {
            return East;
        }

        if (West != null && West != cameFrom)
        {
            return West;
        }

        throw new Exception("can't move!");
    }
}
