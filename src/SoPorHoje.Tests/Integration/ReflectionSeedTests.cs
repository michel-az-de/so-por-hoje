using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Integration;

/// <summary>
/// Integration tests for the reflection seeding pipeline using the real JSON data file.
/// </summary>
public class ReflectionSeedTests : IAsyncLifetime
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
    public async Task SeedFromRealJson_Creates365Entries()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        File.Exists(jsonPath).Should().BeTrue(
            "the real reflection JSON file must exist in TestData");

        await using var stream = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream);

        var count = await _sut.GetCountAsync();
        count.Should().Be(365, "the reflection file should have exactly 365 entries");
    }

    [Fact]
    public async Task SeedFromRealJson_AllDateKeysAreUniqueAndCorrectFormat()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath)) return;

        await using var stream = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream);

        var db = await _db.Database.GetConnectionAsync();
        var reflections = await db.Table<SoPorHoje.Core.Models.DailyReflection>().ToListAsync();

        reflections.Should().HaveCount(365);

        var dateKeys = reflections.Select(r => r.DateKey).ToList();
        dateKeys.Should().OnlyHaveUniqueItems("each DateKey must be unique");
        dateKeys.Should().AllSatisfy(key =>
            key.Should().MatchRegex(@"^\d{2}-\d{2}$", "DateKey must be in MM-dd format"));
    }

    [Fact]
    public async Task SeedFromRealJson_NoFieldIsEmpty()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath)) return;

        await using var stream = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream);

        var db = await _db.Database.GetConnectionAsync();
        var reflections = await db.Table<SoPorHoje.Core.Models.DailyReflection>().ToListAsync();

        reflections.Should().AllSatisfy(r =>
        {
            r.Title.Should().NotBeNullOrWhiteSpace($"Title is empty for DateKey {r.DateKey}");
            r.Quote.Should().NotBeNullOrWhiteSpace($"Quote is empty for DateKey {r.DateKey}");
            r.Text.Should().NotBeNullOrWhiteSpace($"Text is empty for DateKey {r.DateKey}");
        });
    }

    [Fact]
    public async Task SeedFromRealJson_GetTodaysReflection_ReturnsCorrectEntry()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath)) return;

        await using var stream = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream);

        var todayKey = DateTime.Now.ToString("MM-dd");
        var result = await _sut.GetTodaysReflectionAsync();

        result.Should().NotBeNull($"there should be a reflection for today's key '{todayKey}'");
        result!.DateKey.Should().Be(todayKey);
    }

    [Fact]
    public async Task SeedFromRealJson_CalledTwice_DoesNotDuplicate()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "TestData", "daily_reflections_pt-br.json");
        if (!File.Exists(jsonPath)) return;

        await using var stream1 = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream1);
        var count1 = await _sut.GetCountAsync();

        await using var stream2 = File.OpenRead(jsonPath);
        await _sut.SeedFromJsonAsync(stream2);
        var count2 = await _sut.GetCountAsync();

        count2.Should().Be(count1);
    }
}
