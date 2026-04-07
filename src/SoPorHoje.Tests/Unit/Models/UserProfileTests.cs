using FluentAssertions;
using SoPorHoje.Core.Models;

namespace SoPorHoje.Tests.Unit.Models;

public class UserProfileTests
{
    [Fact]
    public void SoberDays_WhenSobrietyDateIsToday_ReturnsZero()
    {
        var profile = new UserProfile { SobrietyDate = DateTime.Today };
        profile.SoberDays.Should().Be(0);
    }

    [Fact]
    public void SoberDays_WhenSobrietyDateIs90DaysAgo_Returns90()
    {
        var profile = new UserProfile { SobrietyDate = DateTime.Today.AddDays(-90) };
        profile.SoberDays.Should().Be(90);
    }

    [Fact]
    public void SoberDays_WhenSobrietyDateIsFuture_ReturnsZero()
    {
        var profile = new UserProfile { SobrietyDate = DateTime.Today.AddDays(5) };
        profile.SoberDays.Should().Be(0);
    }

    [Fact]
    public void SoberDays_WhenSobrietyDateIsOneYearAgo_Returns365Or366()
    {
        var profile = new UserProfile { SobrietyDate = DateTime.Today.AddYears(-1) };
        profile.SoberDays.Should().BeInRange(365, 366);
    }

    [Fact]
    public void DeviceId_IsGeneratedAutomatically()
    {
        var profile = new UserProfile();
        profile.DeviceId.Should().NotBeNullOrEmpty();
        Guid.TryParse(profile.DeviceId, out _).Should().BeTrue();
    }

    [Fact]
    public void DeviceId_IsDifferentForEachInstance()
    {
        var profile1 = new UserProfile();
        var profile2 = new UserProfile();
        profile1.DeviceId.Should().NotBe(profile2.DeviceId);
    }
}
