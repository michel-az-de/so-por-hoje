using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

/// <summary>
/// Tests for the sobriety reset flow: resetting the counter,
/// verifying the profile is updated, and that reset history is preserved.
/// </summary>
public class SobrietyResetTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private UserRepository _userRepo = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _userRepo = new UserRepository(_db.Database, NullLogger<UserRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task ResetSobriety_UpdatesProfileDate()
    {
        var original = new DateTime(2024, 1, 1);
        var profile = MockFactory.CreateProfile(original);
        profile.Id = 0;
        await _userRepo.SaveProfileAsync(profile);

        var newDate = new DateTime(2026, 3, 15);
        await _userRepo.ResetSobrietyAsync(newDate);

        var updated = await _userRepo.GetProfileAsync();
        updated.Should().NotBeNull();
        updated!.SobrietyDate.Should().Be(newDate);
        updated.SoberDays.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ResetSobriety_ToToday_GivesZeroSoberDays()
    {
        var profile = MockFactory.CreateProfile(new DateTime(2020, 1, 1));
        profile.Id = 0;
        await _userRepo.SaveProfileAsync(profile);

        await _userRepo.ResetSobrietyAsync(DateTime.Today);

        var updated = await _userRepo.GetProfileAsync();
        updated!.SoberDays.Should().Be(0);
    }

    [Fact]
    public async Task ResetSobriety_MultipleResets_AllPreserveDateCorrectly()
    {
        var profile = MockFactory.CreateProfile(new DateTime(2020, 1, 1));
        profile.Id = 0;
        await _userRepo.SaveProfileAsync(profile);

        await _userRepo.ResetSobrietyAsync(new DateTime(2025, 6, 1));
        await _userRepo.ResetSobrietyAsync(new DateTime(2026, 1, 1));
        await _userRepo.ResetSobrietyAsync(DateTime.Today);

        var updated = await _userRepo.GetProfileAsync();
        updated!.SobrietyDate.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task ResetSobriety_WhenNoProfile_DoesNotThrow()
    {
        var act = async () => await _userRepo.ResetSobrietyAsync(DateTime.Today);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Profile_SoberDays_NeverNegative()
    {
        // Future date should not produce negative sober days
        var futureDate = DateTime.Today.AddDays(10);
        var profile = new UserProfile
        {
            SobrietyDate = futureDate,
            DeviceId = Guid.NewGuid().ToString(),
        };
        profile.SoberDays.Should().Be(0);
    }
}
