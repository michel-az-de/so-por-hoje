using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IChipService
{
    List<SobrietyChip> GetAllChips();
    SobrietyChip GetCurrentChip(int soberDays);
    SobrietyChip? GetNextChip(int soberDays);
    double GetProgressToNext(int soberDays);
    int GetDaysUntilNext(int soberDays);
    Task<List<ChipEarnedEvent>> GetUncelebratedAsync(int soberDays);
    Task MarkCelebratedAsync(int chipRequiredDays);
    int GetEarnedCount(int soberDays);
}
