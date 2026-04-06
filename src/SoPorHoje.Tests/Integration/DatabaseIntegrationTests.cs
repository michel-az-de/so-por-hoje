using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Core.Models;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Tests.Helpers;

namespace SoPorHoje.Tests.Integration;

/// <summary>
/// Tests the full SQLite database lifecycle: schema creation, data persistence,
/// cross-repository operations, and sobriety reset flow.
/// </summary>
public class DatabaseIntegrationTests : IAsyncLifetime
{
    private TestDatabase _db = null!;

    public async Task InitializeAsync()
    {
        _db = new TestDatabase();
        await _db.InitAsync();
    }

    public async Task DisposeAsync() => await _db.DisposeAsync();

    [Fact]
    public async Task FullFlow_CreateProfileAndPledge_BothPersist()
    {
        var userRepo = new UserRepository(_db.Database, NullLogger<UserRepository>.Instance);
        var pledgeRepo = new PledgeRepository(_db.Database, NullLogger<PledgeRepository>.Instance);

        // 1. Create profile
        var profile = new UserProfile
        {
            SobrietyDate = new DateTime(2024, 6, 15),
            PersonalReason = "Pela minha família",
        };
        await userRepo.SaveProfileAsync(profile);

        // 2. Make a pledge
        var pledge = MockFactory.CreatePledge();
        await pledgeRepo.SavePledgeAsync(pledge);

        // 3. Verify both persisted
        var savedProfile = await userRepo.GetProfileAsync();
        savedProfile.Should().NotBeNull();
        savedProfile!.PersonalReason.Should().Be("Pela minha família");

        var savedPledge = await pledgeRepo.GetTodaysPledgeAsync();
        savedPledge.Should().NotBeNull();
    }

    [Fact]
    public async Task FullFlow_SeedReflectionsAndQuery_Works()
    {
        var reflectionRepo = new ReflectionRepository(_db.Database, NullLogger<ReflectionRepository>.Instance);

        // Seed 3 reflections
        var json = """
            [
              {"date":"2025-04-06","title":"ACEITAÇÃO","quote":"Citação 1.","text":"Texto 1.","content":"Ref 1"},
              {"date":"2025-04-07","title":"GRATIDÃO","quote":"Citação 2.","text":"Texto 2.","content":"Ref 2"},
              {"date":"2025-04-08","title":"HUMILDADE","quote":"Citação 3.","text":"Texto 3.","content":"Ref 3"}
            ]
            """;
        await reflectionRepo.SeedFromJsonAsync(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)));

        // Query
        var count = await reflectionRepo.GetCountAsync();
        count.Should().Be(3);

        var reflection = await reflectionRepo.GetByDateKeyAsync("04-06");
        reflection.Should().NotBeNull();
        reflection!.Title.Should().Be("ACEITAÇÃO");
    }

    [Fact]
    public async Task FullFlow_SobrietyReset_CreatesResetEventAndUpdatesDate()
    {
        var userRepo = new UserRepository(_db.Database, NullLogger<UserRepository>.Instance);

        var profile = new UserProfile
        {
            SobrietyDate = new DateTime(2024, 1, 1),
        };
        await userRepo.SaveProfileAsync(profile);

        // Reset sobriety
        var newDate = new DateTime(2026, 3, 15);
        await userRepo.ResetSobrietyAsync(newDate);

        // Verify profile updated
        var updated = await userRepo.GetProfileAsync();
        updated.Should().NotBeNull();
        updated!.SobrietyDate.Should().Be(newDate);

        // Verify reset event was created
        var db = await _db.Database.GetConnectionAsync();
        var resetEvents = await db.Table<ResetEvent>().ToListAsync();
        resetEvents.Should().HaveCount(1);
        resetEvents[0].PreviousSobrietyDate.Should().Be(new DateTime(2024, 1, 1));
        resetEvents[0].NewSobrietyDate.Should().Be(newDate);
    }

    [Fact]
    public async Task FullFlow_MultipleRepositoriesShareDatabase_DataIsConsistent()
    {
        var userRepo = new UserRepository(_db.Database, NullLogger<UserRepository>.Instance);
        var pledgeRepo = new PledgeRepository(_db.Database, NullLogger<PledgeRepository>.Instance);
        var meetingRepo = new MeetingRepository(_db.Database, NullLogger<MeetingRepository>.Instance);

        // Insert data from different repos
        await userRepo.SaveProfileAsync(new UserProfile { SobrietyDate = DateTime.Today.AddDays(-100) });
        await pledgeRepo.SavePledgeAsync(MockFactory.CreatePledge());
        await meetingRepo.UpsertAsync(new List<OnlineMeeting> { MockFactory.CreateMeeting() });

        // Verify all data persists correctly
        (await userRepo.HasProfileAsync()).Should().BeTrue();
        (await pledgeRepo.GetTodaysPledgeAsync()).Should().NotBeNull();
        (await meetingRepo.GetAllAsync()).Should().HaveCount(1);
    }

    [Fact]
    public async Task FullFlow_PledgeStreak_CalculatesCorrectly()
    {
        var pledgeRepo = new PledgeRepository(_db.Database, NullLogger<PledgeRepository>.Instance);

        // Create 7 consecutive days
        for (int i = 0; i < 7; i++)
            await pledgeRepo.SavePledgeAsync(MockFactory.CreatePledge(DateTime.Today.AddDays(-i)));

        var streak = await pledgeRepo.GetStreakAsync();
        streak.Should().Be(7);

        var total = await pledgeRepo.GetTotalPledgesAsync();
        total.Should().Be(7);
    }
}
