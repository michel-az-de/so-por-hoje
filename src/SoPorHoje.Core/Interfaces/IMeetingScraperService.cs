using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IMeetingScraperService
{
    Task<List<OnlineMeeting>> ScrapeAsync(CancellationToken ct = default);
}
