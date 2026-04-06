using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Refit;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Data.Remote;
using SoPorHoje.Data.Remote.DTOs;
using SoPorHoje.Data.Sync;
using TestHelpers = SoPorHoje.Tests.Helpers.MockFactory;

namespace SoPorHoje.Tests.Integration;

/// <summary>
/// Tests the SyncEngine: push local data to API, pull remote data, offline fallback.
/// </summary>
public class SyncFlowTests : IAsyncLifetime
{
    private TestDatabase _db = null!;
    private Mock<IApiClient> _apiClientMock = null!;
    private SyncEngine _sut = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
        _apiClientMock = new Mock<IApiClient>();
        _sut = new SyncEngine(_db.Database, _apiClientMock.Object, NullLogger<SyncEngine>.Instance);
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    private static ApiResponse<T> SuccessResponse<T>(T content) =>
        new(new HttpResponseMessage(HttpStatusCode.OK), content, new RefitSettings());

    private static ApiResponse<T> FailResponse<T>() =>
        new(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable), default, new RefitSettings());

    [Fact]
    public async Task SyncAsync_WhenOffline_SkipsSync()
    {
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ReturnsAsync(FailResponse<string>());

        // Should complete without pushing or pulling
        await _sut.SyncAsync();

        _apiClientMock.Verify(a => a.PushAsync(It.IsAny<SyncPushRequest>()), Times.Never);
        _apiClientMock.Verify(a => a.GetMeetingsAsync(), Times.Never);
    }

    [Fact]
    public async Task IsOnlineAsync_WhenHealthCheckSucceeds_ReturnsTrue()
    {
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ReturnsAsync(SuccessResponse<string>("ok"));

        var result = await _sut.IsOnlineAsync();
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsOnlineAsync_WhenHealthCheckFails_ReturnsFalse()
    {
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var result = await _sut.IsOnlineAsync();
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SyncAsync_WhenOnlineWithUnsyncedPledges_PushesAndMarksAsSynced()
    {
        // Setup online
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ReturnsAsync(SuccessResponse<string>("ok"));

        _apiClientMock
            .Setup(a => a.PushAsync(It.IsAny<SyncPushRequest>()))
            .ReturnsAsync(SuccessResponse(new SyncPushResponse { Success = true }));

        _apiClientMock
            .Setup(a => a.GetMeetingsAsync())
            .ReturnsAsync(SuccessResponse(new List<MeetingDto>()));

        _apiClientMock
            .Setup(a => a.GetReflectionsAsync())
            .ReturnsAsync(SuccessResponse(new List<ReflectionDto>()));

        // Insert unsynced pledge
        var pledgeRepo = new PledgeRepository(_db.Database, NullLogger<PledgeRepository>.Instance);
        var pledge = TestHelpers.CreatePledge();
        pledge.IsSynced = false;
        await pledgeRepo.SavePledgeAsync(pledge);

        await _sut.SyncAsync();

        // Verify push was called
        _apiClientMock.Verify(a => a.PushAsync(It.IsAny<SyncPushRequest>()), Times.Once);

        // Verify pledge is now marked as synced
        var db = await _db.Database.GetConnectionAsync();
        var updatedPledges = await db.Table<DailyPledge>().ToListAsync();
        updatedPledges.Should().AllSatisfy(p => p.IsSynced.Should().BeTrue());
    }

    [Fact]
    public async Task SyncAsync_WhenApiPullsNewMeetings_MeetingsAreStored()
    {
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ReturnsAsync(SuccessResponse<string>("ok"));

        _apiClientMock
            .Setup(a => a.PushAsync(It.IsAny<SyncPushRequest>()))
            .ReturnsAsync(SuccessResponse(new SyncPushResponse { Success = true }));

        _apiClientMock
            .Setup(a => a.GetMeetingsAsync())
            .ReturnsAsync(SuccessResponse(new List<MeetingDto>
            {
                new()
                {
                    GroupName = "Grupo Sync",
                    DaysOfWeekMask = 127,
                    StartTimeTicks = new TimeSpan(19, 0, 0).Ticks,
                    EndTimeTicks = new TimeSpan(20, 30, 0).Ticks,
                    MeetingUrl = "https://zoom.us/j/sync",
                    Platform = "Zoom",
                    Source = "api",
                    LastScrapedAt = DateTime.UtcNow,
                },
            }));

        _apiClientMock
            .Setup(a => a.GetReflectionsAsync())
            .ReturnsAsync(SuccessResponse(new List<ReflectionDto>()));

        await _sut.SyncAsync();

        var meetingRepo = new MeetingRepository(_db.Database, NullLogger<MeetingRepository>.Instance);
        var meetings = await meetingRepo.GetAllAsync();
        meetings.Should().HaveCount(1);
        meetings[0].GroupName.Should().Be("Grupo Sync");
    }

    [Fact]
    public async Task SyncAsync_WhenApiThrows_FailsSilently()
    {
        _apiClientMock
            .Setup(a => a.HealthCheckAsync())
            .ReturnsAsync(SuccessResponse<string>("ok"));

        _apiClientMock
            .Setup(a => a.PushAsync(It.IsAny<SyncPushRequest>()))
            .ThrowsAsync(new HttpRequestException("API unreachable"));

        var act = async () => await _sut.SyncAsync();
        await act.Should().NotThrowAsync();
    }
}
