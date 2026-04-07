using FluentAssertions;
using SoPorHoje.Core.Models;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Models;

/// <summary>
/// Tests for OnlineMeeting live-detection logic with specific time/day combinations.
/// </summary>
public class LiveMeetingTests
{
    [Fact]
    public void IsLiveNow_DuringMeeting_ReturnsTrue()
    {
        var now = DateTime.Now;
        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = 127, // all days
            StartTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(-30)).Ticks,
            EndTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(30)).Ticks,
        };

        meeting.IsLiveNow.Should().BeTrue();
    }

    [Fact]
    public void IsLiveNow_BeforeMeeting_ReturnsFalse()
    {
        var now = DateTime.Now;
        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = 127,
            StartTimeTicks = now.TimeOfDay.Add(TimeSpan.FromHours(2)).Ticks,
            EndTimeTicks = now.TimeOfDay.Add(TimeSpan.FromHours(3)).Ticks,
        };

        meeting.IsLiveNow.Should().BeFalse();
    }

    [Fact]
    public void IsLiveNow_AfterMeeting_ReturnsFalse()
    {
        var now = DateTime.Now;
        // Only works if it's not past midnight already
        if (now.TimeOfDay < TimeSpan.FromHours(2)) return;

        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = 127,
            StartTimeTicks = now.TimeOfDay.Add(TimeSpan.FromHours(-3)).Ticks,
            EndTimeTicks = now.TimeOfDay.Add(TimeSpan.FromHours(-1)).Ticks,
        };

        meeting.IsLiveNow.Should().BeFalse();
    }

    [Fact]
    public void IsLiveNow_WrongDay_ReturnsFalse()
    {
        var now = DateTime.Now;
        // DaysMask has only the bit for a day OTHER than today
        var todayBit = 1 << (int)now.DayOfWeek;
        var otherDayMask = (127 ^ todayBit) & 127; // all days except today

        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = otherDayMask,
            StartTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(-30)).Ticks,
            EndTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(30)).Ticks,
        };

        meeting.IsLiveNow.Should().BeFalse();
    }

    [Fact]
    public void MinutesUntilStart_BeforeMeeting_ReturnsPositive()
    {
        var now = DateTime.Now;
        var start = now.TimeOfDay.Add(TimeSpan.FromMinutes(45));

        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = 127,
            StartTimeTicks = start.Ticks,
            EndTimeTicks = start.Add(TimeSpan.FromHours(1)).Ticks,
        };

        meeting.MinutesUntilStart.Should().BePositive();
        meeting.MinutesUntilStart.Should().BeLessThanOrEqualTo(46);
    }

    [Fact]
    public void MinutesUntilStart_DuringMeeting_ReturnsNull()
    {
        var now = DateTime.Now;

        var meeting = new OnlineMeeting
        {
            DaysOfWeekMask = 127,
            StartTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(-30)).Ticks,
            EndTimeTicks = now.TimeOfDay.Add(TimeSpan.FromMinutes(30)).Ticks,
        };

        meeting.MinutesUntilStart.Should().BeNull();
    }

    [Fact]
    public void IsLiveNow_AllDaysMask127_TodayDuringMeeting_ReturnsTrue()
    {
        var now = DateTime.Now;
        var meeting = MockFactory.CreateMeeting(
            start: now.TimeOfDay.Subtract(TimeSpan.FromMinutes(10)),
            end: now.TimeOfDay.Add(TimeSpan.FromMinutes(50)),
            daysMask: 127);

        meeting.IsLiveNow.Should().BeTrue();
    }
}
