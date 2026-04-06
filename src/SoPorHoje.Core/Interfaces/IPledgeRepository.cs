using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IPledgeRepository
{
    Task<DailyPledge?> GetTodaysPledgeAsync();
    Task SavePledgeAsync(DailyPledge pledge);
    Task<List<DailyPledge>> GetHistoryAsync(int days = 30);
    Task<int> GetStreakAsync();
    Task<int> GetTotalPledgesAsync();
}
