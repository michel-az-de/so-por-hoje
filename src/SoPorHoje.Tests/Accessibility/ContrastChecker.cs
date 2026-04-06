namespace SoPorHoje.Tests.Accessibility;

/// <summary>
/// WCAG 2.1 contrast ratio calculator.
/// Minimum AA: 4.5:1 for normal text, 3:1 for large text (18pt+ or 14pt bold).
/// </summary>
public static class ContrastChecker
{
    /// <summary>
    /// Calculates WCAG contrast ratio between two hex colors.
    /// </summary>
    public static double GetContrastRatio(string fgHex, string bgHex)
    {
        var (r1, g1, b1) = ParseHex(fgHex);
        var (r2, g2, b2) = ParseHex(bgHex);
        var l1 = GetRelativeLuminance(r1, g1, b1);
        var l2 = GetRelativeLuminance(r2, g2, b2);
        var lighter = Math.Max(l1, l2);
        var darker = Math.Min(l1, l2);
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>Returns true if the contrast meets WCAG AA for normal text (4.5:1).</summary>
    public static bool MeetsAA(string fg, string bg) => GetContrastRatio(fg, bg) >= 4.5;

    /// <summary>Returns true if the contrast meets WCAG AA for large text (3:1).</summary>
    public static bool MeetsAALarge(string fg, string bg) => GetContrastRatio(fg, bg) >= 3.0;

    private static (double R, double G, double B) ParseHex(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 3)
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";

        var r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255.0;
        var g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255.0;
        var b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255.0;
        return (r, g, b);
    }

    private static double GetRelativeLuminance(double r, double g, double b)
    {
        double R = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        double G = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        double B = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);
        return 0.2126 * R + 0.7152 * G + 0.0722 * B;
    }
}
