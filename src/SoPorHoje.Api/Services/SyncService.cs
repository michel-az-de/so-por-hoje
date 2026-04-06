using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Data.Entities;
using SoPorHoje.Api.DTOs;

namespace SoPorHoje.Api.Services;

public class SyncService(AppDbContext db, ILogger<SyncService> logger)
{
    public async Task<(Guid UserId, bool IsNew)> GetOrCreateUserAsync(string deviceId, CancellationToken ct = default)
    {
        var existing = await db.Users.FirstOrDefaultAsync(u => u.DeviceId == deviceId, ct);
        if (existing is not null)
            return (existing.Id, false);

        var user = new UserEntity
        {
            DeviceId = deviceId,
            SobrietyDate = DateOnly.FromDateTime(DateTime.UtcNow),
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Novo usuário criado: {UserId}", user.Id);
        return (user.Id, true);
    }

    public async Task<SyncPushResponse> PushAsync(SyncPushRequest request, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.DeviceId == request.DeviceId, ct);
        if (user is null)
        {
            user = new UserEntity { DeviceId = request.DeviceId, SobrietyDate = DateOnly.FromDateTime(DateTime.UtcNow) };
            db.Users.Add(user);
            await db.SaveChangesAsync(ct);
        }

        // Atualizar perfil
        if (request.Profile is not null)
        {
            if (DateOnly.TryParse(request.Profile.SobrietyDate, out var sobrietyDate))
                user.SobrietyDate = sobrietyDate;
            user.PersonalReason = request.Profile.PersonalReason;
            user.UpdatedAt = DateTimeOffset.UtcNow;
        }

        // Upsert pledges
        var syncedPledgeIds = new List<Guid>();
        foreach (var dto in request.Pledges ?? [])
        {
            if (!DateOnly.TryParse(dto.PledgeDate, out var pledgeDate)) continue;

            var existing = await db.Pledges
                .FirstOrDefaultAsync(p => p.UserId == user.Id && p.PledgeDate == pledgeDate, ct);

            if (existing is null)
            {
                existing = new PledgeEntity
                {
                    UserId = user.Id,
                    PledgeDate = pledgeDate,
                    PledgedAt = dto.PledgedAt,
                    Fulfilled = dto.Fulfilled,
                    Notes = dto.Notes,
                };
                db.Pledges.Add(existing);
            }
            else
            {
                existing.Fulfilled = dto.Fulfilled;
                existing.Notes = dto.Notes;
            }

            await db.SaveChangesAsync(ct);
            syncedPledgeIds.Add(existing.Id);
        }

        // Upsert chip events
        var syncedChipEventIds = new List<Guid>();
        foreach (var dto in request.ChipEvents ?? [])
        {
            var existing = await db.ChipEvents.FirstOrDefaultAsync(
                c => c.UserId == user.Id && c.ChipRequiredDays == dto.ChipRequiredDays && c.EarnedAt == dto.EarnedAt, ct);

            if (existing is null)
            {
                existing = new ChipEventEntity
                {
                    UserId = user.Id,
                    ChipRequiredDays = dto.ChipRequiredDays,
                    EarnedAt = dto.EarnedAt,
                };
                db.ChipEvents.Add(existing);
                await db.SaveChangesAsync(ct);
            }

            syncedChipEventIds.Add(existing.Id);
        }

        // Upsert reset events
        foreach (var dto in request.ResetEvents ?? [])
        {
            if (!DateOnly.TryParse(dto.PreviousSobrietyDate, out var prevDate)) continue;
            if (!DateOnly.TryParse(dto.NewSobrietyDate, out var newDate)) continue;

            var alreadyExists = await db.ResetEvents.AnyAsync(
                r => r.UserId == user.Id && r.OccurredAt == dto.OccurredAt, ct);

            if (!alreadyExists)
            {
                db.ResetEvents.Add(new ResetEventEntity
                {
                    UserId = user.Id,
                    PreviousSobrietyDate = prevDate,
                    NewSobrietyDate = newDate,
                    DaysAccumulated = dto.DaysAccumulated,
                    OccurredAt = dto.OccurredAt,
                });
                await db.SaveChangesAsync(ct);
            }
        }

        return new SyncPushResponse(syncedPledgeIds, syncedChipEventIds, DateTimeOffset.UtcNow);
    }

    public async Task DeleteUserAsync(string deviceId, CancellationToken ct = default)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.DeviceId == deviceId, ct);
        if (user is null) return;

        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Usuário {DeviceId} removido permanentemente", deviceId);
    }
}
