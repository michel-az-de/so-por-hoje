using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

public class ReflectionRepositoryTests : IAsyncLifetime
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
    public async Task SeedFromJson_CreatesEntries()
    {
        var json = """
            [
              {"date":"2025-04-06","title":"ACEITAÇÃO","quote":"Citação teste.","text":"Texto teste.","content":"ALCOÓLICOS ANÔNIMOS, p. 1"},
              {"date":"2025-04-07","title":"GRATIDÃO","quote":"Citação 2.","text":"Texto 2.","content":"DOZE PASSOS, p. 2"},
              {"date":"2025-04-08","title":"HUMILDADE","quote":"Citação 3.","text":"Texto 3.","content":"VIVENDO SÓBRIO, p. 3"}
            ]
            """;

        await _sut.SeedFromJsonAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)));

        var count = await _sut.GetCountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task SeedFromJson_DoesNotDuplicateOnSecondCall()
    {
        var json = """
            [{"date":"2025-01-01","title":"TÍTULO","quote":"Q","text":"T","content":"Ref"}]
            """;

        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        await _sut.SeedFromJsonAsync(new MemoryStream(bytes));
        var count1 = await _sut.GetCountAsync();

        await _sut.SeedFromJsonAsync(new MemoryStream(bytes));
        var count2 = await _sut.GetCountAsync();

        count2.Should().Be(count1);
    }

    [Fact]
    public async Task GetTodaysReflection_AfterSeed_ReturnsReflection()
    {
        var todayKey = DateTime.Now.ToString("MM-dd");
        var db = await _db.Database.GetConnectionAsync();
        await db.InsertAsync(MockFactory.CreateReflection(todayKey));

        var result = await _sut.GetTodaysReflectionAsync();
        result.Should().NotBeNull();
        result!.DateKey.Should().Be(todayKey);
        result.Title.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetByDateKey_InvalidKey_ReturnsNull()
    {
        var result = await _sut.GetByDateKeyAsync("13-32");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByDateKey_ExistingKey_ReturnsReflection()
    {
        var db = await _db.Database.GetConnectionAsync();
        await db.InsertAsync(MockFactory.CreateReflection("01-15"));

        var result = await _sut.GetByDateKeyAsync("01-15");
        result.Should().NotBeNull();
        result!.DateKey.Should().Be("01-15");
    }

    [Fact]
    public async Task GetCount_EmptyDatabase_ReturnsZero()
    {
        var count = await _sut.GetCountAsync();
        count.Should().Be(0);
    }
}
