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
              {"date":"2025-04-06","title":"Aceitação","quote":"Citação teste.","text":"Texto teste.","theme":"aceitacao","license":"CC-BY-SA-4.0","author":"Projeto Só Por Hoje"},
              {"date":"2025-04-07","title":"Gratidão","quote":"Citação 2.","text":"Texto 2.","theme":"gratidao","license":"CC-BY-SA-4.0","author":"Projeto Só Por Hoje"},
              {"date":"2025-04-08","title":"Humildade","quote":"Citação 3.","text":"Texto 3.","theme":"humildade","license":"CC-BY-SA-4.0","author":"Projeto Só Por Hoje"}
            ]
            """;

        await _sut.SeedFromJsonAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)));

        var count = await _sut.GetCountAsync();
        count.Should().Be(3);

        // Metadados de licença/autoria/tema devem ser persistidos (atribuição CC BY-SA).
        var first = await _sut.GetByDateKeyAsync("04-06");
        first.Should().NotBeNull();
        first!.Theme.Should().Be("aceitacao");
        first.License.Should().Be("CC-BY-SA-4.0");
        first.Author.Should().Be("Projeto Só Por Hoje");
    }

    [Fact]
    public async Task SeedFromJson_DoesNotDuplicateOnSecondCall()
    {
        var json = """
            [{"date":"2025-01-01","title":"Título","quote":"Q","text":"T","theme":"tema","license":"CC-BY-SA-4.0","author":"Projeto Só Por Hoje"}]
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
