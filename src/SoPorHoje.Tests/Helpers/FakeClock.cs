namespace SoPorHoje.Tests.Helpers;

public interface IClock
{
    DateTime Now { get; }
    DateTime Today { get; }
    DateTime UtcNow { get; }
}

public class FakeClock : IClock
{
    public DateTime Now { get; set; } = new(2026, 4, 6, 14, 30, 0);
    public DateTime Today => Now.Date;
    public DateTime UtcNow => Now.ToUniversalTime();

    public FakeClock At(int year, int month, int day, int hour = 0, int minute = 0)
    {
        Now = new DateTime(year, month, day, hour, minute, 0);
        return this;
    }

    public FakeClock AtTime(int hour, int minute)
    {
        Now = new DateTime(Now.Year, Now.Month, Now.Day, hour, minute, 0);
        return this;
    }
}
