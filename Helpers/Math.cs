namespace Helpers;

public class Math
{
    public static long LowestCommonMultiple(IEnumerable<long> numbers)
    {
        return numbers.Aggregate(LowestCommonMultiple);
    }
    public static long LowestCommonMultiple(long a, long b)
    {
        return System.Math.Abs(a * b) / GreatestCommonDivisor(a, b);
    }
    public static long GreatestCommonDivisor(long a, long b)
    {
        return b == 0 ? a : GreatestCommonDivisor(b, a % b);
    }
}
