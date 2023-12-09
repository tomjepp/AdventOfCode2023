using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var grid = new Dictionary<Point, Item>();
var numbers = new List<Number>();

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

        int column = 0;

        while (column < line.Length)
        {
            if (line[column] == '.')
            {
                column++;
                continue;
            }

            if (char.IsDigit(line[column]))
            {
                var points = new List<Point>();

                int length = 0;
                while (column + length < line.Length && char.IsDigit(line[column + length]))
                {
                    points.Add(new Point() { X = column + length, Y = row });
                    length++;
                }

                var value = int.Parse(line.Substring(column, length));
                var number = new Number(grid, points, value);
                foreach (var point in points)
                {
                    grid.Add(point, number);
                }
                numbers.Add(number);
                column += length;
            }
            else
            {
                var point = new Point()
                {
                    X = column,
                    Y = row,
                };
                var symbol = new Symbol()
                {
                    Point = point,
                    Value = line[column],
                };
                grid.Add(point, symbol);
                column++;
            }
        }

        row++;
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

int sum = numbers.Where(x => x.IsAdjacentToSymbol()).Sum(x => x.Value);
Console.WriteLine(sum);

stopwatch.Stop();
var processedIn = stopwatch.Elapsed;

Console.WriteLine();
Console.WriteLine($"parsing time: {parsedIn.TotalMilliseconds:0.####} milliseconds");
Console.WriteLine($"processing time: {processedIn.TotalMilliseconds:0.####} milliseconds");

struct Point
{
    public int X;
    public int Y;
}

abstract class Item;

class Symbol : Item
{
    public Point Point { get; set; }
    public char Value { get; set; }
}

class Number(Dictionary<Point, Item> grid, List<Point> points, int value)
    : Item
{
    private Dictionary<Point, Item> Grid { get; } = grid;
    private List<Point> Points { get; } = points;
    public int Value { get; } = value;

    public bool IsAdjacentToSymbol()
    {
        var processedPoints = new List<Point>();

        foreach (var ourPoint in Points)
        {
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    var point = new Point()
                    {
                        X = ourPoint.X + xOffset,
                        Y = ourPoint.Y + yOffset,
                    };

                    if (Points.Contains(point) || processedPoints.Contains(point))
                    {
                        continue;
                    }

                    if (!Grid.ContainsKey(point))
                    {
                        continue;
                    }

                    if (Grid[point] is Symbol)
                    {
                        return true;
                    }

                    processedPoints.Add(point);
                }
            }
        }

        return false;
    }
}
