using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

public class PledgeRepositoryTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private PledgeRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new PledgeRepository(_db.Database, NullLogger<PledgeRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task GetTodaysPledge_WhenNoPledge_ReturnsNull()
    {
        var result = await _sut.GetTodaysPledgeAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTodaysPledge_WhenPledgeExists_ReturnsPledge()
    {
        var pledge = MockFactory.CreatePledge();
        await _sut.SavePledgeAsync(pledge);

        var result = await _sut.GetTodaysPledgeAsync();
        result.Should().NotBeNull();
        result!.PledgeDate.Date.Should().Be(DateTime.Today);
    }

    [Fact]
    public async Task GetStreak_NoPledges_ReturnsZero()
    {
        var streak = await _sut.GetStreakAsync();
        streak.Should().Be(0);
    }

    [Fact]
    public async Task GetStreak_ConsecutiveDays_ReturnsCorrectCount()
    {
        for (int i = 0; i < 5; i++)
            await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-i)));

        var streak = await _sut.GetStreakAsync();
        streak.Should().Be(5);
    }

    [Fact]
    public async Task GetStreak_GapInMiddle_ReturnsStreakSinceLastGap()
    {
        await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today));
        await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-1)));
        // Skip -2
        await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-3)));
        await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-4)));

        var streak = await _sut.GetStreakAsync();
        streak.Should().Be(2);
    }

    [Fact]
    public async Task GetTotalPledges_ReturnsCorrectCount()
    {
        for (int i = 0; i < 10; i++)
            await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-i)));

        var total = await _sut.GetTotalPledgesAsync();
        total.Should().Be(10);
    }

    [Fact]
    public async Task GetHistory_ReturnsRecordsWithinRange()
    {
        for (int i = 0; i < 10; i++)
            await _sut.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-i)));

        var history = await _sut.GetHistoryAsync(days: 30);
        history.Should().HaveCount(10);
        history.Should().BeInDescendingOrder(p => p.PledgeDate);
    }

    [Fact]
    public async Task SavePledge_TwiceForSameDay_UpdatesExistingPledge()
    {
        var pledge = MockFactory.CreatePledge();
        await _sut.SavePledgeAsync(pledge);
        pledge.Notes = "Atualizado";
        await _sut.SavePledgeAsync(pledge);

        var total = await _sut.GetTotalPledgesAsync();
        total.Should().Be(1);
    }
}
