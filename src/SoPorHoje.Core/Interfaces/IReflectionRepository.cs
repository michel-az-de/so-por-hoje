using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IReflectionRepository
{
    Task<DailyReflection?> GetTodaysReflectionAsync();
    Task<DailyReflection?> GetByDateKeyAsync(string dateKey);
    Task SeedFromJsonAsync(Stream jsonStream);
    Task<int> GetCountAsync();
    Task<List<DailyReflection>> GetAllAsync();
}
