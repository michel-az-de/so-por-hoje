using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Repositories;

public class MeetingRepositoryTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private MeetingRepository _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _sut = new MeetingRepository(_db.Database, NullLogger<MeetingRepository>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task GetAll_EmptyDatabase_ReturnsEmptyList()
    {
        var result = await _sut.GetAllAsync();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_AfterInsert_ReturnsAllMeetings()
    {
        var meetings = new List<OnlineMeeting>
        {
            MockFactory.CreateMeeting("Grupo A"),
            MockFactory.CreateMeeting("Grupo B"),
            MockFactory.CreateMeeting("Grupo C"),
        };

        await _sut.UpsertAsync(meetings);

        var result = await _sut.GetAllAsync();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetLiveNow_FiltersByCurrentTimeAndDay()
    {
        var now = DateTime.Now;
        var todayBit = 1 << (int)now.DayOfWeek;

        var liveMeeting = MockFactory.CreateMeeting(
            "Reunião Ao Vivo",
            start: now.TimeOfDay.Subtract(TimeSpan.FromMinutes(5)),
            end: now.TimeOfDay.Add(TimeSpan.FromMinutes(30)),
            daysMask: todayBit);

        var futureMeeting = MockFactory.CreateMeeting(
            "Reunião Futura",
            start: now.TimeOfDay.Add(TimeSpan.FromHours(2)),
            end: now.TimeOfDay.Add(TimeSpan.FromHours(3)),
            daysMask: todayBit);

        await _sut.UpsertAsync(new List<OnlineMeeting> { liveMeeting, futureMeeting });

        var live = await _sut.GetLiveNowAsync();
        live.Should().HaveCount(1);
        live[0].GroupName.Should().Be("Reunião Ao Vivo");
    }

    [Fact]
    public async Task GetNext_ReturnsEarliestUpcomingMeeting()
    {
        var now = DateTime.Now;
        var todayBit = 1 << (int)now.DayOfWeek;

        var closer = MockFactory.CreateMeeting(
            "Mais Cedo",
            start: now.TimeOfDay.Add(TimeSpan.FromMinutes(30)),
            end: now.TimeOfDay.Add(TimeSpan.FromMinutes(90)),
            daysMask: todayBit);

        var later = MockFactory.CreateMeeting(
            "Mais Tarde",
            start: now.TimeOfDay.Add(TimeSpan.FromHours(2)),
            end: now.TimeOfDay.Add(TimeSpan.FromHours(3)),
            daysMask: todayBit);

        await _sut.UpsertAsync(new List<OnlineMeeting> { later, closer });

        var next = await _sut.GetNextAsync();
        next.Should().NotBeNull();
        next!.GroupName.Should().Be("Mais Cedo");
    }

    [Fact]
    public async Task Upsert_UpdatesExistingMeeting()
    {
        var meeting = MockFactory.CreateMeeting("Grupo Original");
        await _sut.UpsertAsync(new List<OnlineMeeting> { meeting });

        var inserted = (await _sut.GetAllAsync()).First();
        inserted.GroupName = "Grupo Atualizado";
        await _sut.UpsertAsync(new List<OnlineMeeting> { inserted });

        var all = await _sut.GetAllAsync();
        all.Should().HaveCount(1);
        all[0].GroupName.Should().Be("Grupo Atualizado");
    }

    [Fact]
    public async Task GetLastScrapeTime_WhenNoMeetings_ReturnsNull()
    {
        var result = await _sut.GetLastScrapeTimeAsync();
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetLastScrapeTime_WhenMeetingsExist_ReturnsMostRecent()
    {
        var older = MockFactory.CreateMeeting("Grupo Antigo");
        older.LastScrapedAt = DateTime.UtcNow.AddDays(-5);

        var newer = MockFactory.CreateMeeting("Grupo Novo");
        newer.LastScrapedAt = DateTime.UtcNow.AddDays(-1);

        await _sut.UpsertAsync(new List<OnlineMeeting> { older, newer });

        var lastScrape = await _sut.GetLastScrapeTimeAsync();
        lastScrape.Should().NotBeNull();
        lastScrape!.Value.Date.Should().Be(newer.LastScrapedAt.Date);
    }
}
