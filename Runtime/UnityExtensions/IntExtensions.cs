public static class IntExtensions
{
    public static string ToStringWithUnitPrefixes(this int value)
    {
        if (value >= 1000000000)
            return $"{(float)value / 10000000000:0.0}G";
        if (value >= 1000000)
            return $"{(float)value / 10000000:0.0}M";
        if (value >= 1000)
            return $"{(float)value / 1000:0.0}K";
        return value.ToString();
    }
}