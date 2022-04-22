public static class IntExtensions
{
    public static string ToStringWithUnitPrefixes(this int value, string format = "G")
    {
        if (value >= 1000000000)
            return $"{((float)value / 10000000000).ToString(format)}G";
        if (value >= 1000000)
            return $"{((float)value / 10000000).ToString(format)}M";
        if (value >= 1000)
            return $"{((float)value / 1000).ToString(format)}K";
        return value.ToString();
    }
}