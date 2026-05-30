using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Scraper;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Unit.Scraper;

public class IntergruposScraperTests
{
    [Fact]
    public async Task Scrape_WhenSiteReturnsValidJson_ReturnsMeetings()
    {
        // The scraper tries JSON API endpoints first. Return valid JSON for any request.
        const string json = """
            [
              {
                "name": "Grupo Esperança Online",
                "url": "https://zoom.us/j/123456789",
                "start_time": "19:00",
                "end_time": "20:30",
                "days": "segunda,quarta,sexta"
              },
              {
                "name": "Grupo Serenidade",
                "url": "https://meet.google.com/abc-def-ghi",
                "start_time": "08:00",
                "end_time": "09:00",
                "days": "todos os dias"
              }
            ]
            """;

        var handler = new MockHttpMessageHandler(content: json);
        var httpClient = new HttpClient(handler);
        var scraper = new IntergruposScraper(httpClient, NullLogger<IntergruposScraper>.Instance);

        var meetings = await scraper.ScrapeAsync();

        meetings.Should().NotBeEmpty();
        meetings.Should().AllSatisfy(m =>
        {
            m.GroupName.Should().NotBeNullOrEmpty();
            m.MeetingUrl.Should().NotBeNullOrEmpty();
            m.DaysOfWeekMask.Should().BeGreaterThan(0);
        });
    }

    [Fact]
    public async Task Scrape_WhenSiteDown_ReturnsEmptyList()
    {
        // Throwing HttpRequestException causes the scraper to return empty list
        var handler = new MockHttpMessageHandler(exception: new HttpRequestException("Conexão recusada"));
        var httpClient = new HttpClient(handler);
        var scraper = new IntergruposScraper(httpClient, NullLogger<IntergruposScraper>.Instance);

        var meetings = await scraper.ScrapeAsync();

        meetings.Should().BeEmpty();
    }

    [Fact]
    public async Task Scrape_WhenTimeout_ReturnsEmptyList()
    {
        // TaskCanceledException without user cancellation = timeout
        var handler = new MockHttpMessageHandler(
            exception: new TaskCanceledException("Timeout simulado"));
        var httpClient = new HttpClient(handler);
        var scraper = new IntergruposScraper(httpClient, NullLogger<IntergruposScraper>.Instance);

        var meetings = await scraper.ScrapeAsync();

        meetings.Should().BeEmpty();
    }

    [Theory]
    [InlineData("todos os dias", 127)]
    [InlineData("diariamente", 127)]
    [InlineData("segunda", 2)]
    [InlineData("terça", 4)]
    [InlineData("quarta", 8)]
    [InlineData("quinta", 16)]
    [InlineData("sexta", 32)]
    [InlineData("sábado", 64)]
    [InlineData("domingo", 1)]
    [InlineData("segunda,quarta,sexta", 2 | 8 | 32)]
    [InlineData("", 0)]
    [InlineData(null, 0)]
    public void ParseDaysMask_ReturnsCorrectBitmask(string? text, int expectedMask)
    {
        var mask = IntergruposScraper.ParseDaysMask(text);
        mask.Should().Be(expectedMask);
    }

    [Fact]
    public async Task Scrape_WithEmptyJsonArray_ReturnsEmptyList()
    {
        var handler = new MockHttpMessageHandler(content: "[]");
        var httpClient = new HttpClient(handler);
        var scraper = new IntergruposScraper(httpClient, NullLogger<IntergruposScraper>.Instance);

        // This will try JSON API (empty), then HTML scraping — HTML is also empty
        // Status 200 with empty JSON array → moves to HTML scraping → no links found
        var meetings = await scraper.ScrapeAsync();

        meetings.Should().BeEmpty();
    }

    [Fact]
    public async Task Scrape_WhenApiReturnsValidJson_PlatformIsDetected()
    {
        const string json = """
            [{"name":"Grupo Zoom","url":"https://zoom.us/j/9999","start_time":"19:00","end_time":"20:30","days":"segunda"}]
            """;

        var handler = new MockHttpMessageHandler(content: json);
        var httpClient = new HttpClient(handler);
        var scraper = new IntergruposScraper(httpClient, NullLogger<IntergruposScraper>.Instance);

        var meetings = await scraper.ScrapeAsync();

        meetings.Should().HaveCount(1);
        meetings[0].Platform.Should().Be("Zoom");
    }
}
