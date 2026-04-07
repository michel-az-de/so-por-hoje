using SoPorHoje.Core.Models;

namespace SoPorHoje.Tests.Helpers;

public static class MockFactory
{
    public static UserProfile CreateProfile(DateTime? sobrietyDate = null)
        => new()
        {
            Id = 1,
            SobrietyDate = sobrietyDate ?? new DateTime(2025, 6, 15),
            PersonalReason = "Pela minha família",
            DeviceId = Guid.NewGuid().ToString(),
        };

    public static DailyPledge CreatePledge(DateTime? date = null)
        => new()
        {
            PledgeDate = date ?? DateTime.Today,
            PledgedAt = DateTime.UtcNow,
        };

    public static OnlineMeeting CreateMeeting(
        string name = "Grupo Teste",
        TimeSpan? start = null,
        TimeSpan? end = null,
        int daysMask = 127)
        => new()
        {
            GroupName = name,
            DaysOfWeekMask = daysMask,
            StartTimeTicks = (start ?? new TimeSpan(19, 0, 0)).Ticks,
            EndTimeTicks = (end ?? new TimeSpan(20, 30, 0)).Ticks,
            MeetingUrl = "https://zoom.us/j/test",
            Platform = "Zoom",
        };

    public static DailyReflection CreateReflection(string dateKey = "04-06")
        => new()
        {
            DateKey = dateKey,
            Title = "TÍTULO TESTE",
            Quote = "Citação de teste para verificação.",
            Text = "Texto reflexivo completo para testes unitários.",
            Reference = "ALCOÓLICOS ANÔNIMOS, p. 99",
        };
}
