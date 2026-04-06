using SoPorHoje.Core.Models;

namespace SoPorHoje.Core.Interfaces;

public interface IUserRepository
{
    Task<UserProfile?> GetProfileAsync();
    Task SaveProfileAsync(UserProfile profile);
    Task<bool> HasProfileAsync();
    Task ResetSobrietyAsync(DateTime newDate);
}
