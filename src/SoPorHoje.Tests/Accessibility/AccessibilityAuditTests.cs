using FluentAssertions;

namespace SoPorHoje.Tests.Accessibility;

/// <summary>
/// WCAG 2.1 accessibility audit for the Só Por Hoje app color palette.
/// Colors sourced from src/SoPorHoje.App/Resources/Styles/Colors.xaml.
///
/// Light theme palette:
///   BgPrimary:     #F5F0E8  (cream background)
///   BgSurface:     #FFFFFF  (white surface)
///   TextPrimary:   #1A1A1A  (almost black)
///   TextSecondary: #4A4A4A  (dark gray)
///   TextMuted:     #888888  (medium gray)
///   Accent:        #2E5BB8  (AA blue)
///   Warm:          #C8963E  (golden)
///   Success:       #28A745  (green)
///   Danger:        #DC3545  (red)
/// </summary>
public class AccessibilityAuditTests
{
    // Normal text (4.5:1 minimum)
    [Theory]
    [InlineData("#1A1A1A", "#F5F0E8", 4.5, "TextPrimary on BgPrimary")]
    [InlineData("#1A1A1A", "#FFFFFF", 4.5, "TextPrimary on BgSurface")]
    [InlineData("#4A4A4A", "#F5F0E8", 4.5, "TextSecondary on BgPrimary")]
    [InlineData("#4A4A4A", "#FFFFFF", 4.5, "TextSecondary on BgSurface")]
    [InlineData("#FFFFFF", "#2E5BB8", 4.5, "White text on Accent")]
    public void NormalText_MeetsWCAG_AA(string fgHex, string bgHex, double minRatio, string description)
    {
        var ratio = ContrastChecker.GetContrastRatio(fgHex, bgHex);
        ratio.Should().BeGreaterOrEqualTo(minRatio,
            $"{description}: contrast {fgHex} on {bgHex} = {ratio:F2}:1, minimum {minRatio}:1");
    }

    // Large text (3:1 minimum — applies to headings 18pt+ and 14pt bold)
    [Theory]
    [InlineData("#DC3545", "#FFFFFF", 3.0, "Danger on BgSurface (large text, SOS button)")]
    [InlineData("#2E5BB8", "#F5F0E8", 3.0, "Accent on BgPrimary (large text, pledge button)")]
    [InlineData("#2E5BB8", "#FFFFFF", 3.0, "Accent on BgSurface (large text)")]
    public void LargeText_MeetsWCAG_AA_Large(string fgHex, string bgHex, double minRatio, string description)
    {
        var ratio = ContrastChecker.GetContrastRatio(fgHex, bgHex);
        ratio.Should().BeGreaterOrEqualTo(minRatio,
            $"{description}: contrast {fgHex} on {bgHex} = {ratio:F2}:1, minimum {minRatio}:1");
    }

    // Document colors that do NOT currently meet WCAG AA Large (3:1) — these need design review
    [Theory]
    [InlineData("#C8963E", "#F5F0E8", "Warm on BgPrimary — below 3:1")]
    [InlineData("#C8963E", "#FFFFFF", "Warm on BgSurface — below 3:1")]
    [InlineData("#28A745", "#F5F0E8", "Success on BgPrimary — below 3:1")]
    public void KnownAccessibilityGap_ColorsDoNotMeetAALarge(string fgHex, string bgHex, string description)
    {
        var ratio = ContrastChecker.GetContrastRatio(fgHex, bgHex);
        // Document actual ratio — these are known gaps that should be improved in the color palette
        ratio.Should().BeLessThan(3.0,
            $"{description}: {fgHex} on {bgHex} = {ratio:F2}:1 (below 3.0 threshold)");
    }

    [Fact]
    public void ContrastRatio_BlackOnWhite_Is21To1()
    {
        var ratio = ContrastChecker.GetContrastRatio("#000000", "#FFFFFF");
        ratio.Should().BeApproximately(21.0, 0.1);
    }

    [Fact]
    public void ContrastRatio_WhiteOnWhite_Is1To1()
    {
        var ratio = ContrastChecker.GetContrastRatio("#FFFFFF", "#FFFFFF");
        ratio.Should().BeApproximately(1.0, 0.01);
    }

    [Fact]
    public void MeetsAA_BlackOnWhite_ReturnsTrue()
    {
        ContrastChecker.MeetsAA("#000000", "#FFFFFF").Should().BeTrue();
    }

    [Fact]
    public void MeetsAA_WhiteOnWhite_ReturnsFalse()
    {
        ContrastChecker.MeetsAA("#FFFFFF", "#FFFFFF").Should().BeFalse();
    }

    [Fact]
    public void MeetsAALarge_LowContrastColors_ReturnsFalse()
    {
        // Very similar colors — should not meet even the large-text threshold
        ContrastChecker.MeetsAALarge("#AAAAAA", "#BBBBBB").Should().BeFalse();
    }

    [Fact]
    public void ContrastRatio_IsSymmetric()
    {
        var fg = "#1A1A1A";
        var bg = "#F5F0E8";
        var ratio1 = ContrastChecker.GetContrastRatio(fg, bg);
        var ratio2 = ContrastChecker.GetContrastRatio(bg, fg);
        ratio1.Should().BeApproximately(ratio2, 0.001);
    }
}
