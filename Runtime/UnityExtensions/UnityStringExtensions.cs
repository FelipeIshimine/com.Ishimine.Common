using UnityEngine;

public static class UnityStringExtensions
{
    public enum StringColor
    {
        Black,
        Blue,
        Brown,
        Cyan,
        Darkblue,
        Green,
        Grey,
        LightBlue,
        Lime,
        Magenta,
        Maroon,
        Navy,
        Olive,
        Orange,
        Purple,
        Red,
        Silver,
        Teal,
        White,
        Yellow
    }

    public static readonly string[] ColorCodes = new []
    {
        "#000000ff",
        "#0000ffff",
        "#a52a2aff",
        "#00ffffff",
        "#0000a0ff",
        "#008000ff",
        "#808080ff",
        "#add8e6ff",
        "#00ff00ff",
        "#ff00ffff",
        "#800000ff",
        "#000080ff",
        "#808000ff",
        "#ffa500ff",
        "#800080ff",
        "#ff0000ff",
        "#c0c0c0ff",
        "#008080ff",
        "#ffffffff",
        "#ffff00ff"
    };

    public static string ApplyColor(this string source, Color color)=> $"<color={ColorUtility.ToHtmlStringRGB(color)}>{source}</color>";

    public static string ApplyColor(this string source, StringColor stringColor)=> $"<color={ColorCodes[(int)stringColor]}>{source}</color>";
    public static string ApplyConditionalColor(this string source, StringColor trueStringColor, StringColor falseStringColor, bool value)=> ApplyColor(source, value ? trueStringColor : falseStringColor);
    
    
}