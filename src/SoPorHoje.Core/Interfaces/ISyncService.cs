namespace SoPorHoje.Core.Interfaces;

public interface ISyncService
{
    Task SyncAsync(CancellationToken ct = default);
    Task<bool> IsOnlineAsync();
}
