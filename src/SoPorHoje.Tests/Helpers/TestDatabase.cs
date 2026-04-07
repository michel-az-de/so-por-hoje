using Microsoft.Extensions.Logging.Abstractions;
using SoPorHoje.Data.Local;

namespace SoPorHoje.Tests.Helpers;

/// <summary>
/// Creates an isolated SoPorHojeDatabase backed by a temporary file for each test.
/// Each instance gets its own DB file, providing full test isolation.
/// </summary>
public class TestDatabase : IAsyncDisposable
{
    private readonly string _dbPath;

    public SoPorHojeDatabase Database { get; }

    public TestDatabase()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"soporhoje_test_{Guid.NewGuid():N}.db");
        Database = new SoPorHojeDatabase(_dbPath, NullLogger<SoPorHojeDatabase>.Instance);
    }

    public async Task InitAsync()
    {
        // Calling GetConnectionAsync triggers schema creation and chip seeding
        await Database.GetConnectionAsync();
    }

    public async ValueTask DisposeAsync()
    {
        var conn = await Database.GetConnectionAsync();
        await conn.CloseAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }
}
