using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

/// <summary>
/// Tests for daily reflection retrieval by date key.
/// </summary>
public class DailyReflectionTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private ReflectionRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new ReflectionRepository(_db.Database, NullLogger<ReflectionRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task GetByDateKey_ExistingKey_ReturnsReflection()
    {
        var reflection = MockFactory.CreateReflection("07-04");
        var conn = await _db.Database.GetConnectionAsync();
        await conn.InsertAsync(reflection);

        var result = await _sut.GetByDateKeyAsync("07-04");

        result.Should().NotBeNull();
        result!.DateKey.Should().Be("07-04");
        result.Title.Should().Be(reflection.Title);
    }

    [Fact]
    public async Task GetByDateKey_NonExistingKey_ReturnsNull()
    {
        var result = await _sut.GetByDateKeyAsync("99-99");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTodaysReflection_UsesMMddFormat()
    {
        var today = DateTime.Today;
        var dateKey = today.ToString("MM-dd");
        var reflection = new DailyReflection
        {
            DateKey = dateKey,
            Title = "Reflexão de Hoje",
            Quote = "Citação teste",
            Text = "Texto reflexivo do dia.",
            Reference = "ALCOÓLICOS ANÔNIMOS, p. 1",
        };
        var conn = await _db.Database.GetConnectionAsync();
        await conn.InsertAsync(reflection);

        var result = await _sut.GetTodaysReflectionAsync();

        result.Should().NotBeNull();
        result!.DateKey.Should().Be(dateKey);
    }

    [Fact]
    public async Task GetTodaysReflection_WhenNoEntry_ReturnsNull()
    {
        var result = await _sut.GetTodaysReflectionAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCount_AfterSeed_Returns365()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath))
        {
            // Skip if test data not available
            return;
        }
        using var stream = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream);

        var count = await _sut.GetCountAsync();
        count.Should().Be(365);
    }

    [Fact]
    public async Task SeedFromJson_IsIdempotent_DoesNotDuplicate()
    {
        var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath)) return;

        using var stream1 = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream1);

        using var stream2 = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream2);

        var count = await _sut.GetCountAsync();
        count.Should().Be(365);
    }

    [Theory]
    [InlineData("01-01")]
    [InlineData("06-15")]
    [InlineData("12-31")]
    public async Task GetByDateKey_VariousKeys_ReturnsCorrectFormat(string dateKey)
    {
        var reflection = new DailyReflection
        {
            DateKey = dateKey,
            Title = $"Reflexão {dateKey}",
            Quote = "Citação",
            Text = "Texto",
            Reference = "Ref",
        };
        var conn = await _db.Database.GetConnectionAsync();
        await conn.InsertAsync(reflection);

        var result = await _sut.GetByDateKeyAsync(dateKey);
        result.Should().NotBeNull();
        result!.DateKey.Should().Be(dateKey);
    }
}
