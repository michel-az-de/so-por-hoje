namespace SoPorHoje.Scraper;

public record ScrapedMeeting(
    string GroupName,
    int DaysOfWeekMask,
    TimeSpan StartTime,
    TimeSpan EndTime,
    string MeetingUrl,
    string Platform
);
