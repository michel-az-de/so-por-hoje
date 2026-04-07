using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Services;

/// <summary>
/// Tests for the chip celebration flow: earning an uncelebrated chip,
/// marking it as celebrated, and verifying it doesn't appear again.
/// </summary>
public class ChipCelebrationTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private ChipService _chipService = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _chipService = new ChipService(_db.Database, NullLogger<ChipService>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task GetUncelebrated_WhenFirstChipJustEarned_ReturnsOneEvent()
    {
        // 1 sober day → earns Amarela (requiredDays = 1)
        var uncelebrated = await _chipService.GetUncelebratedAsync(soberDays: 1);

        uncelebrated.Should().HaveCount(1);
        uncelebrated[0].ChipRequiredDays.Should().Be(1);
        uncelebrated[0].CelebrationShown.Should().BeFalse();
    }

    [Fact]
    public async Task GetUncelebrated_AfterMarkCelebrated_ReturnsEmpty()
    {
        // Earn Amarela
        await _chipService.GetUncelebratedAsync(soberDays: 1);

        // Mark as celebrated
        await _chipService.MarkCelebratedAsync(chipRequiredDays: 1);

        // Should not appear again
        var uncelebrated = await _chipService.GetUncelebratedAsync(soberDays: 1);
        uncelebrated.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUncelebrated_MultipleChipsEarned_ReturnsAll()
    {
        // 365 days → earns Amarela, Azul, Rosa, Vermelha, Verde (5 chips)
        var uncelebrated = await _chipService.GetUncelebratedAsync(soberDays: 365);

        uncelebrated.Should().HaveCount(5);
    }

    [Fact]
    public async Task MarkCelebrated_IsIdempotent()
    {
        await _chipService.GetUncelebratedAsync(soberDays: 1);
        await _chipService.MarkCelebratedAsync(chipRequiredDays: 1);
        await _chipService.MarkCelebratedAsync(chipRequiredDays: 1); // second call should not throw

        var uncelebrated = await _chipService.GetUncelebratedAsync(soberDays: 1);
        uncelebrated.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUncelebrated_CalledTwice_DoesNotDuplicate()
    {
        var first = await _chipService.GetUncelebratedAsync(soberDays: 90);
        var second = await _chipService.GetUncelebratedAsync(soberDays: 90);

        first.Count.Should().Be(second.Count);
    }
}
