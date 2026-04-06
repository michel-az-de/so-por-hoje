using Microsoft.EntityFrameworkCore;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Endpoints;
using SoPorHoje.Api.Services;
using SoPorHoje.Scraper;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ────────────────────────────────────────────────────────────────

// PostgreSQL via EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP client com User-Agent para o scraper
builder.Services.AddHttpClient<IntergruposScraper>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("SoPorHoje-App/1.0 (+soporhoje)");
    client.Timeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddSingleton<IntergruposScraper>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var http = factory.CreateClient(nameof(IntergruposScraper));
    var logger = sp.GetRequiredService<ILogger<IntergruposScraper>>();
    return new IntergruposScraper(http, logger);
});

// Domain services
builder.Services.AddScoped<SyncService>();

// Background job do scraper
builder.Services.AddHostedService<ScraperHostedService>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Só Por Hoje API", Version = "v1" });
});

// CORS — restrito ao domínio do app em produção
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Contains("*"))
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        else
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
    });
});

// ─── Build ────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ─── Middleware ───────────────────────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// ─── Endpoints ────────────────────────────────────────────────────────────────

app.MapAuthEndpoints();
app.MapSyncEndpoints();
app.MapMeetingEndpoints();
app.MapReflectionEndpoints();
app.MapHealthEndpoints();

// ─── Database setup + Seed ────────────────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await db.Database.MigrateAsync();
        logger.LogInformation("Migrations aplicadas com sucesso");

        // Resolve data path: configurable, then alongside the executable, then ContentRoot
        var configPath = app.Configuration["ReflectionsDataPath"];
        var reflectionsPath = !string.IsNullOrWhiteSpace(configPath)
            ? configPath
            : FindDataFile("daily_reflections_pt-br.json", app.Environment.ContentRootPath);
        await ReflectionSeeder.SeedAsync(db, reflectionsPath, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao aplicar migrations ou seed");
    }
}

app.Run();

// Tornar Program acessível para testes de integração
public partial class Program
{
    /// <summary>
    /// Procura o arquivo de dados em vários locais possíveis:
    /// 1. {contentRoot}/data/{fileName}
    /// 2. {executableDir}/data/{fileName}
    /// 3. {currentDir}/data/{fileName}
    /// </summary>
    private static string FindDataFile(string fileName, string contentRoot)
    {
        string[] candidates =
        [
            Path.Combine(contentRoot, "data", fileName),
            Path.Combine(AppContext.BaseDirectory, "data", fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "data", fileName),
        ];
        return Array.Find(candidates, File.Exists) ?? candidates[0];
    }
}
