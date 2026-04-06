using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Services;

public class ChipServiceTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private ChipService _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new ChipService(_db.Database, NullLogger<ChipService>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Theory]
    [InlineData(0, 1, "Amarela")]
    [InlineData(1, 1, "Amarela")]
    [InlineData(89, 1, "Amarela")]
    [InlineData(90, 90, "Azul")]
    [InlineData(179, 90, "Azul")]
    [InlineData(180, 180, "Rosa")]
    [InlineData(270, 270, "Vermelha")]
    [InlineData(365, 365, "Verde")]
    [InlineData(730, 730, "Verde Gravata")]
    [InlineData(1825, 1825, "Branca Gravata")]
    [InlineData(3650, 3650, "Amarela Gravata")]
    [InlineData(5475, 5475, "Azul Gravata")]
    [InlineData(7300, 7300, "Rosa Gravata")]
    [InlineData(9999, 7300, "Rosa Gravata")]
    public void GetCurrentChip_ReturnsCorrectChip(int days, int expectedReq, string expectedName)
    {
        var chip = _sut.GetCurrentChip(days);
        chip.RequiredDays.Should().Be(expectedReq);
        chip.Name.Should().Be(expectedName);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 90)]
    [InlineData(90, 180)]
    [InlineData(7300, null)]
    public void GetNextChip_ReturnsCorrectChip(int days, int? expectedReq)
    {
        var next = _sut.GetNextChip(days);
        if (expectedReq == null)
            next.Should().BeNull();
        else
            next!.RequiredDays.Should().Be(expectedReq.Value);
    }

    [Theory]
    [InlineData(0, 1.0)]   // Before first chip: current=Amarela, next=Amarela, range=0 → 1.0
    [InlineData(45, 0.5)]
    [InlineData(89, 0.989)]
    [InlineData(90, 0.0)]
    public void GetProgressToNext_ReturnsCorrectProgress(int days, double expected)
    {
        var progress = _sut.GetProgressToNext(days);
        progress.Should().BeApproximately(expected, 0.05);
    }

    [Fact]
    public void GetAllChips_Returns10Chips()
    {
        _sut.GetAllChips().Should().HaveCount(10);
    }

    [Fact]
    public void GetAllChips_IsOrderedByRequiredDays()
    {
        var chips = _sut.GetAllChips();
        chips.Should().BeInAscendingOrder(c => c.RequiredDays);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(90, 2)]
    [InlineData(365, 5)]
    [InlineData(7300, 10)]
    public void GetEarnedCount_ReturnsCorrect(int days, int expected)
    {
        _sut.GetEarnedCount(days).Should().Be(expected);
    }

    [Fact]
    public void GetDaysUntilNext_BeforeFirstChip_Returns1()
    {
        _sut.GetDaysUntilNext(0).Should().Be(1);
    }

    [Fact]
    public void GetDaysUntilNext_AtLastChip_ReturnsZero()
    {
        _sut.GetDaysUntilNext(7300).Should().Be(0);
    }

    [Fact]
    public void GetProgressToNext_AtLastChip_Returns1()
    {
        _sut.GetProgressToNext(7300).Should().Be(1.0);
    }
}
