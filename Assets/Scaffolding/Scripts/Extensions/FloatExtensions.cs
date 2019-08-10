using UnityEngine;

public static partial class FloatExtensions
{
    // These methods break my naming convention of methods always having to be verbs but their
    // use is unambiguous and the whole point of these methods is to make floating point based 
    // comparisons more compact and readable.

    public static bool Equal(this float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    public static bool EqualOrGreater(this float a, float b)
    {
        return Equal(a, b) || a > b;
    }

    public static bool EqualOrSmaller(this float a, float b)
    {
        return Equal(a, b) || a < b;
    }

    public static bool Greater(this float a, float b)
    {
        return !Equal(a, b) && a > b;
    }

    public static bool Smaller(this float a, float b)
    {
        return !Equal(a, b) && a < b;
    }
    
    public static float Decay(this float value, float by)
    {
        if (value.Equal(0.0f))
            return 0.0f;
        
        if (value < 0)
            return value > -by ? 0.0f : Mathf.Min(value + by, 0.0f);
        
        return value < by ? 0.0f : Mathf.Max(value - by, 0.0f);
    }
}
