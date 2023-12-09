using System.Diagnostics;

var stopwatch = new Stopwatch();

stopwatch.Start();

var grid = new Dictionary<Point, Item>();
var gears = new List<Gear>();

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
                var number = new Number(points, value);

                foreach (var point in points)
                {
                    grid.Add(point, number);
                }
                column += length;
            }
            else if (line[column] == '*')
            {
                var point = new Point()
                {
                    X = column,
                    Y = row,
                };
                var gear = new Gear(grid)
                {
                    Point = point,
                    Value = line[column],
                };
                gears.Add(gear);
                column += 1;
            }
            else
            {
                column += 1;
            }
        }

        row++;
    }
}

stopwatch.Stop();
var parsedIn = stopwatch.Elapsed;
stopwatch.Restart();

int sum = gears.Sum(x => x.GetGearRatio());
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

class Number(List<Point> points, int value) : Item
{
    public List<Point> Points { get; } = points;
    public int Value { get; } = value;
}

class Gear(Dictionary<Point, Item> grid) : Symbol
{
    public Dictionary<Point, Item> Grid { get; } = grid;

    public int GetGearRatio()
    {
        var adjacentNumbers = new List<Number>();

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                var point = new Point()
                {
                    X = this.Point.X + xOffset,
                    Y = this.Point.Y + yOffset,
                };

                if (point.Equals(this.Point))
                {
                    continue;
                }

                if (!Grid.ContainsKey(point))
                {
                    continue;
                }

                if (Grid[point] is not Number) continue;
                var number = Grid[point] as Number;
                if (number is null)
                {
                    throw new InvalidOperationException();
                }
                if (!adjacentNumbers.Contains(number))
                {
                    adjacentNumbers.Add(number);
                }

            }
        }

        if (adjacentNumbers.Count != 2)
        {
            return 0;
        }

        return adjacentNumbers[0].Value * adjacentNumbers[1].Value;
    }
}
