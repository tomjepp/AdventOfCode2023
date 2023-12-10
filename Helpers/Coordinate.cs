namespace Helpers;

public struct Coordinate
{
    public int X;
    public int Y;

    public Coordinate Translate(int xOffset, int yOffset)
    {
        return new Coordinate()
        {
            X = this.X + xOffset,
            Y = this.Y + yOffset,
        };
    }

    public Coordinate Up(int step = 1) => Translate(0, -1 * step);
    public Coordinate Down(int step = 1) => Translate(0, 1 * step);
    public Coordinate Left(int step = 1) => Translate(-1 * step, 0);
    public Coordinate Right(int step = 1) => Translate(1 * step, 0);
    public Coordinate East(int step = 1) => Right(step);
    public Coordinate West(int step = 1) => Left(step);
    public Coordinate North(int step = 1) => Up(step);
    public Coordinate South(int step = 1) => Down(step);
}
