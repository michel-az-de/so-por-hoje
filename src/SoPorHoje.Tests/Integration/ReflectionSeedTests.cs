using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Integration;

/// <summary>
/// Integration tests for the reflection seeding pipeline using the project's own
/// authored sample content (CC BY-SA 4.0) — no third-party/protected text.
/// </summary>
public class ReflectionSeedTests : IAsyncLifetime
{
    private const string FixtureName = "reflections-sample-pt-BR.json";

    private TestDatabase _db = null!;
    private ReflectionRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new ReflectionRepository(_db.Database, NullLogger<ReflectionRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    private static string FixturePath =>
        Path.Combine(AppContext.BaseDirectory, "TestData", FixtureName);

    [Fact]
    public async Task SeedFromJson_CreatesEntries()
    {
        File.Exists(FixturePath).Should().BeTrue(
            "the authored sample reflection JSON must be copied to TestData");

        await using var stream = File.OpenRead(FixturePath);
        await _sut.SeedFromJsonAsync(stream);

        var count = await _sut.GetCountAsync();
        count.Should().BeGreaterThanOrEqualTo(14, "the January sample batch has at least 14 entries");
    }

    [Fact]
    public async Task SeedFromJson_AllDateKeysAreUniqueAndCorrectFormat()
    {
        await using var stream = File.OpenRead(FixturePath);
        await _sut.SeedFromJsonAsync(stream);

        var all = await _sut.GetAllAsync();

        var dateKeys = all.Select(r => r.DateKey).ToList();
        dateKeys.Should().OnlyHaveUniqueItems("each DateKey must be unique");
        dateKeys.Should().AllSatisfy(key =>
            key.Should().MatchRegex(@"^\d{2}-\d{2}$", "DateKey must be in MM-dd format"));
    }

    [Fact]
    public async Task SeedFromJson_CoreFieldsAreNotEmpty()
    {
        await using var stream = File.OpenRead(FixturePath);
        await _sut.SeedFromJsonAsync(stream);

        var all = await _sut.GetAllAsync();
        all.Should().AllSatisfy(r =>
        {
            r.Title.Should().NotBeNullOrWhiteSpace($"Title is empty for DateKey {r.DateKey}");
            r.Quote.Should().NotBeNullOrWhiteSpace($"Quote is empty for DateKey {r.DateKey}");
            r.Text.Should().NotBeNullOrWhiteSpace($"Text is empty for DateKey {r.DateKey}");
        });
    }

    [Fact]
    public async Task SeedFromJson_GetByDateKey_ReturnsKnownEntry()
    {
        await using var stream = File.OpenRead(FixturePath);
        await _sut.SeedFromJsonAsync(stream);

        var result = await _sut.GetByDateKeyAsync("01-01");
        result.Should().NotBeNull("the sample batch includes 1 de janeiro");
        result!.Title.Should().Be("Aceitação");
    }

    [Fact]
    public async Task SeedFromJson_CalledTwice_DoesNotDuplicate()
    {
        await using (var stream1 = File.OpenRead(FixturePath))
            await _sut.SeedFromJsonAsync(stream1);
        var count1 = await _sut.GetCountAsync();

        await using (var stream2 = File.OpenRead(FixturePath))
            await _sut.SeedFromJsonAsync(stream2);
        var count2 = await _sut.GetCountAsync();

        count2.Should().Be(count1);
    }
}
