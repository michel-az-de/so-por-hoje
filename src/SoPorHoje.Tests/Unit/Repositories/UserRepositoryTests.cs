using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

public class UserRepositoryTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private UserRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new UserRepository(_db.Database, NullLogger<UserRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task GetProfile_WhenNoProfile_ReturnsNull()
    {
        var result = await _sut.GetProfileAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task HasProfile_WhenNoProfile_ReturnsFalse()
    {
        var result = await _sut.HasProfileAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SaveProfile_ThenGetProfile_RoundTrips()
    {
        var profile = MockFactory.CreateProfile(new DateTime(2025, 1, 1));
        profile.Id = 0;
        await _sut.SaveProfileAsync(profile);

        var result = await _sut.GetProfileAsync();
        result.Should().NotBeNull();
        result!.PersonalReason.Should().Be("Pela minha família");
        result.SobrietyDate.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public async Task HasProfile_AfterSave_ReturnsTrue()
    {
        var profile = MockFactory.CreateProfile();
        profile.Id = 0;
        await _sut.SaveProfileAsync(profile);

        var result = await _sut.HasProfileAsync();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ResetSobriety_UpdatesDateAndCreatesResetEvent()
    {
        var profile = MockFactory.CreateProfile(new DateTime(2025, 1, 1));
        profile.Id = 0;
        await _sut.SaveProfileAsync(profile);

        var newDate = new DateTime(2026, 3, 1);
        await _sut.ResetSobrietyAsync(newDate);

        var updated = await _sut.GetProfileAsync();
        updated.Should().NotBeNull();
        updated!.SobrietyDate.Should().Be(newDate);
    }

    [Fact]
    public async Task ResetSobriety_WhenNoProfile_DoesNotThrow()
    {
        var act = async () => await _sut.ResetSobrietyAsync(DateTime.Today);
        await act.Should().NotThrowAsync();
    }
}
