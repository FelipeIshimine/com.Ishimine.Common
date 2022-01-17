public static class ArrayExtensions
{
    public static T Last<T>(this T[] source) => source[source.Length - 1];
    public static T First<T>(this T[] source) => source[0];
}