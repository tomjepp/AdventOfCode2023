using System.Diagnostics;
using Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var stopwatch = new Stopwatch();

stopwatch.Start();

var grid = new Dictionary<Coordinate, Pipe>();
Pipe? startingPipe = null;

int gridWidth = 0;
int gridHeight = 0;
using (var reader = File.OpenText("input.txt"))
{
    var row = 0;

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        gridWidth = line.Length;

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

            var location = new Coordinate()
            {
                X = column,
                Y = row,
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

    gridHeight = row;
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
startingPipe.IsLoopPiece = true;
var currentPipe = startingPipe.FindNextPipe(null);
while (currentPipe != startingPipe)
{
    currentPipe.IsLoopPiece = true;
    var nextPipe = currentPipe.FindNextPipe(lastPipe);
    lastPipe = currentPipe;
    currentPipe = nextPipe;
}

DrawGrid(gridWidth, gridHeight);

for (var y = 0; y < gridHeight; y++)
{
    int crossedTimes = 0;

    var x = 0;
    bool inside = false;
    while (x < gridWidth)
    {
        var coordinate = new Coordinate()
        {
            X = x,
            Y = y,
        };
        var pipe = grid[coordinate];
        if (pipe.IsLoopPiece)
        {
            pipe.Inside = false;
            switch (pipe.Type)
            {
                case PipeType.Vertical:
                    inside = !inside;
                    x++;
                    break;

                case PipeType.Horizontal:
                    break;

                case PipeType.L:
                case PipeType.F:
                    var nextCoord = coordinate.Right();
                    var nextPiece = grid[nextCoord];
                    while (nextPiece.Type == PipeType.Horizontal)
                    {
                        nextCoord = nextCoord.Right();
                        nextPiece = grid[nextCoord];
                    }

                    if (pipe.Type == PipeType.L && nextPiece.Type == PipeType.Seven || pipe.Type == PipeType.F && nextPiece.Type == PipeType.J)
                    {
                        inside = !inside;
                    }

                    x = nextCoord.X + 1;
                    break;
            }
        }
        else
        {
            pipe.Inside = inside;
            x++;
        }
    }
}

DrawGrid(gridWidth, gridHeight, gridHeight);
Console.WriteLine(grid.Count(x => x.Value.Inside));

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

void DrawGrid(int width, int height, int idx = -1)
{
    using var image = new Image<Rgba32>(width * 3, height * 3);
    for (int gridX = 0; gridX < width; gridX++)
    {
        for (int gridY = 0; gridY < height; gridY++)
        {
            var coordinate = new Coordinate()
            {
                X = gridX,
                Y = gridY,
            };
            Pipe? pipe = null;
            grid.TryGetValue(coordinate, out pipe);

            int imageX = gridX * 3;
            int imageY = gridY * 3;

            var colour = pipe.IsLoopPiece ? Color.Red : Color.Grey;
            var fillColour = pipe.Inside ? Color.CornflowerBlue : Color.DarkBlue;

            switch (pipe.Type)
            {
                case PipeType.Vertical:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = fillColour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = colour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = colour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = fillColour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.Horizontal:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = colour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = fillColour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = fillColour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = colour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.L:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = fillColour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = colour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = fillColour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = colour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.J:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = colour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = colour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = fillColour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = fillColour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.Seven:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = colour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = fillColour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = colour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = fillColour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.F:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = fillColour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = fillColour;
                    image[imageX + 1, imageY + 1] = colour;
                    image[imageX + 1, imageY + 2] = colour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = colour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
                case PipeType.None:
                    image[imageX + 0, imageY + 0] = fillColour;
                    image[imageX + 0, imageY + 1] = fillColour;
                    image[imageX + 0, imageY + 2] = fillColour;
                    image[imageX + 1, imageY + 0] = fillColour;
                    image[imageX + 1, imageY + 1] = fillColour;
                    image[imageX + 1, imageY + 2] = fillColour;
                    image[imageX + 2, imageY + 0] = fillColour;
                    image[imageX + 2, imageY + 1] = fillColour;
                    image[imageX + 2, imageY + 2] = fillColour;
                    break;
            }
        }
    }

    image.Mutate(x => x.Resize(image.Width * 4, image.Height * 4, KnownResamplers.NearestNeighbor));

    if (idx != -1)
    {
        image.Save($"output-{idx}.png");
    }
    else
    {
        image.Save("output.png");
    }
}

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

    public bool IsLoopPiece { get; set; }
    public bool Inside { get; set; }

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
