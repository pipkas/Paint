namespace PaintApp.UI;

public static class ColorParser
{
    public static byte[] ParseColorRGB(string rgb)
    {
        var pattern = @"^rgb\s*\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*\)$";
        var match = System.Text.RegularExpressions.Regex.Match(rgb, pattern);

        return
        [
            byte.Parse(match.Groups[1].Value),
            byte.Parse(match.Groups[2].Value),
            byte.Parse(match.Groups[3].Value),
            255
        ];
    }
}