using FluentAssertions;
using SoPorHoje.Core.Models;

namespace SoPorHoje.Tests.Unit.Models;

public class SobrietyChipTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(89, true)]
    [InlineData(90, true)]
    public void IsEarned_ReturnsCorrectResult(int soberDays, bool expected)
    {
        var chip = new SobrietyChip { RequiredDays = 1 };
        chip.IsEarned(soberDays).Should().Be(expected);
    }

    [Fact]
    public void IsEarned_WhenSoberDaysExactlyMatchRequired_ReturnsTrue()
    {
        var chip = new SobrietyChip { RequiredDays = 90 };
        chip.IsEarned(90).Should().BeTrue();
    }

    [Fact]
    public void IsEarned_WhenSoberDaysOneLessThanRequired_ReturnsFalse()
    {
        var chip = new SobrietyChip { RequiredDays = 90 };
        chip.IsEarned(89).Should().BeFalse();
    }

    [Fact]
    public void IsEarned_WhenSoberDaysZeroAndRequiredZero_ReturnsTrue()
    {
        var chip = new SobrietyChip { RequiredDays = 0 };
        chip.IsEarned(0).Should().BeTrue();
    }
}
