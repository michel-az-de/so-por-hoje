using FluentAssertions;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Models;

public class OnlineMeetingTests
{
    [Fact]
    public void IsLiveNow_DuringMeetingOnCorrectDay_ReturnsTrue()
    {
        var now = DateTime.Now;
        var meeting = MockFactory.CreateMeeting(
            start: new TimeSpan(now.Hour, now.Minute, 0).Subtract(TimeSpan.FromMinutes(5)),
            end: new TimeSpan(now.Hour, now.Minute, 0).Add(TimeSpan.FromMinutes(30)),
            daysMask: 1 << (int)now.DayOfWeek);

        meeting.IsLiveNow.Should().BeTrue();
    }

    [Fact]
    public void IsLiveNow_BeforeMeetingStart_ReturnsFalse()
    {
        var meeting = MockFactory.CreateMeeting(
            start: DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(2)),
            end: DateTime.Now.TimeOfDay.Add(TimeSpan.FromHours(3)),
            daysMask: 1 << (int)DateTime.Now.DayOfWeek);

        meeting.IsLiveNow.Should().BeFalse();
    }

    [Fact]
    public void IsLiveNow_WrongDayOfWeek_ReturnsFalse()
    {
        var wrongDay = ((int)DateTime.Now.DayOfWeek + 3) % 7;
        var meeting = MockFactory.CreateMeeting(
            start: DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(10)),
            end: DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(30)),
            daysMask: 1 << wrongDay);

        meeting.IsLiveNow.Should().BeFalse();
    }

    [Fact]
    public void MinutesUntilStart_BeforeMeeting_ReturnsPositiveMinutes()
    {
        const int minutesAhead = 45;
        var meeting = MockFactory.CreateMeeting(
            start: DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(minutesAhead)),
            end: DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(minutesAhead + 90)),
            daysMask: 1 << (int)DateTime.Now.DayOfWeek);

        meeting.MinutesUntilStart.Should().NotBeNull();
        meeting.MinutesUntilStart!.Value.Should().BeCloseTo(minutesAhead, 1);
    }

    [Fact]
    public void MinutesUntilStart_AfterMeetingStarted_ReturnsNull()
    {
        var meeting = MockFactory.CreateMeeting(
            start: DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(10)),
            end: DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(80)),
            daysMask: 1 << (int)DateTime.Now.DayOfWeek);

        meeting.MinutesUntilStart.Should().BeNull();
    }

    [Fact]
    public void IsLiveNow_AllDaysMask_OnAnyDayDuringMeeting_ReturnsTrue()
    {
        var now = DateTime.Now;
        var meeting = MockFactory.CreateMeeting(
            start: now.TimeOfDay.Subtract(TimeSpan.FromMinutes(5)),
            end: now.TimeOfDay.Add(TimeSpan.FromMinutes(30)),
            daysMask: 127); // all days

        meeting.IsLiveNow.Should().BeTrue();
    }
}
