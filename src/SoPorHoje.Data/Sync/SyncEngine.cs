using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local;
using SoPorHoje.Data.Remote;
using SoPorHoje.Data.Remote.DTOs;

namespace SoPorHoje.Data.Sync;

/// <summary>
/// Sync strategy:
/// 1. App works 100% OFFLINE — SQLite is source of truth.
/// 2. When online, push local data with IsSynced = false.
/// 3. When online, pull updated meetings and reflections.
/// 4. Sync is EVENTUAL and NON-BLOCKING (never blocks app use).
/// 5. Conflict: last-write-wins by UpdatedAt.
/// 6. If API fails, fail silently (log + retry later).
///
/// Push:  UserProfile, DailyPledge, ChipEarnedEvent, ResetEvent
/// Pull:  OnlineMeeting, DailyReflection
/// </summary>
public class SyncEngine : ISyncService
{
    private readonly SoPorHojeDatabase _database;
    private readonly IApiClient _apiClient;
    private readonly ILogger<SyncEngine> _logger;
    private readonly SemaphoreSlim _syncLock = new(1, 1);

    public SyncEngine(
        SoPorHojeDatabase database,
        IApiClient apiClient,
        ILogger<SyncEngine> logger)
    {
        _database = database;
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<bool> IsOnlineAsync()
    {
        try
        {
            var response = await _apiClient.HealthCheckAsync();
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Health check failed — device appears offline");
            return false;
        }
    }

    public async Task SyncAsync(CancellationToken ct = default)
    {
        if (!await _syncLock.WaitAsync(0, ct))
        {
            _logger.LogDebug("Sync already in progress, skipping");
            return;
        }

        try
        {
            if (!await IsOnlineAsync())
            {
                _logger.LogInformation("Device is offline, skipping sync");
                return;
            }

            await PushLocalDataAsync(ct);
            await PullRemoteDataAsync(ct);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Sync cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sync failed — will retry later");
        }
        finally
        {
            _syncLock.Release();
        }
    }

    private async Task PushLocalDataAsync(CancellationToken ct)
    {
        var db = await _database.GetConnectionAsync();

        var profile = await db.Table<UserProfile>().FirstOrDefaultAsync();
        var pledges = await db.Table<DailyPledge>().Where(p => !p.IsSynced).ToListAsync();
        var chipEvents = await db.Table<ChipEarnedEvent>().Where(e => !e.IsSynced).ToListAsync();
        var resetEvents = await db.Table<ResetEvent>().Where(r => !r.IsSynced).ToListAsync();

        if (profile == null && pledges.Count == 0 && chipEvents.Count == 0 && resetEvents.Count == 0)
        {
            _logger.LogDebug("Nothing to push");
            return;
        }

        var request = new SyncPushRequest
        {
            DeviceId = profile?.DeviceId ?? string.Empty,
            Profile = profile,
            Pledges = pledges,
            ChipEvents = chipEvents,
            ResetEvents = resetEvents,
        };

        ct.ThrowIfCancellationRequested();
        var response = await _apiClient.PushAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Push failed with status {Status}", response.StatusCode);
            return;
        }

        // Mark as synced
        foreach (var pledge in pledges)
        {
            pledge.IsSynced = true;
            await db.UpdateAsync(pledge);
        }
        foreach (var evt in chipEvents)
        {
            evt.IsSynced = true;
            await db.UpdateAsync(evt);
        }
        foreach (var evt in resetEvents)
        {
            evt.IsSynced = true;
            await db.UpdateAsync(evt);
        }

        _logger.LogInformation("Pushed {Pledges} pledges, {Chips} chip events, {Resets} reset events",
            pledges.Count, chipEvents.Count, resetEvents.Count);
    }

    private async Task PullRemoteDataAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        await PullMeetingsAsync();
        ct.ThrowIfCancellationRequested();
        await PullReflectionsAsync();
    }

    private async Task PullMeetingsAsync()
    {
        var response = await _apiClient.GetMeetingsAsync();
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            _logger.LogWarning("Failed to pull meetings: {Status}", response.StatusCode);
            return;
        }

        var db = await _database.GetConnectionAsync();
        var dtos = response.Content;

        // Clear and replace strategy for meetings (server is source of truth)
        await db.DeleteAllAsync<OnlineMeeting>();
        var meetings = dtos.Select(dto => new OnlineMeeting
        {
            GroupName = dto.GroupName,
            DaysOfWeekMask = dto.DaysOfWeekMask,
            StartTimeTicks = dto.StartTimeTicks,
            EndTimeTicks = dto.EndTimeTicks,
            MeetingUrl = dto.MeetingUrl,
            Platform = dto.Platform,
            Source = dto.Source,
            LastScrapedAt = dto.LastScrapedAt,
        }).ToList();

        await db.InsertAllAsync(meetings);
        _logger.LogInformation("Pulled and replaced {Count} meetings", meetings.Count);
    }

    private async Task PullReflectionsAsync()
    {
        var response = await _apiClient.GetReflectionsAsync();
        if (!response.IsSuccessStatusCode || response.Content == null)
        {
            _logger.LogWarning("Failed to pull reflections: {Status}", response.StatusCode);
            return;
        }

        var db = await _database.GetConnectionAsync();
        var dtos = response.Content;

        foreach (var dto in dtos)
        {
            var existing = await db.Table<DailyReflection>()
                .Where(r => r.DateKey == dto.DateKey)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await db.InsertAsync(new DailyReflection
                {
                    DateKey = dto.DateKey,
                    Title = dto.Title,
                    Quote = dto.Quote,
                    Text = dto.Text,
                    Reference = dto.Reference,
                });
            }
            else
            {
                existing.Title = dto.Title;
                existing.Quote = dto.Quote;
                existing.Text = dto.Text;
                existing.Reference = dto.Reference;
                await db.UpdateAsync(existing);
            }
        }

        _logger.LogInformation("Pulled and upserted {Count} reflections", dtos.Count);
    }
}
