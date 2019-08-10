public static partial class IntExtensions
{
    public static int Modulo(this int x, int divisor)
    {
        return (x % divisor + divisor) % divisor;
    }
}
