using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SoPorHoje.Api.Data;
using SoPorHoje.Api.Endpoints;
using SoPorHoje.Api.Services;
using SoPorHoje.Api.Swagger;
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

// Swagger / OpenAPI — dois documentos: inglês e português
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var contactUrl = new Uri("https://github.com/michel-az-de/so-por-hoje");

    c.SwaggerDoc("v1-en", new OpenApiInfo
    {
        Title = "Só Por Hoje API",
        Version = "v1",
        Description =
            "REST API for the **Só Por Hoje** (Just For Today) app — a daily recovery companion " +
            "for Alcoholics Anonymous members in Brazil.\n\n" +
            "## Architecture\n\n" +
            "- **Runtime:** ASP.NET Core 8 Minimal API\n" +
            "- **Database:** PostgreSQL 16 + Entity Framework Core 8\n" +
            "- **Scraper:** Background service collecting AA online meetings every 30 minutes\n" +
            "- **Auth:** Anonymous UUID-based (no email/password, no personal data)\n\n" +
            "## Business Rules\n\n" +
            "- The app is **offline-first**: SQLite on the device is the source of truth; the server is a backup/sync layer.\n" +
            "- All datetimes are UTC on the wire; display is converted to Brasília time (America/Sao_Paulo).\n" +
            "- `DaysOfWeekMask` bitmask: Sunday=1, Monday=2, Tuesday=4, Wednesday=8, Thursday=16, Friday=32, Saturday=64.\n" +
            "- Sobriety chips follow the Brazilian AA standard: Yellow (1d), Blue (90d), Pink (180d), Red (270d), Green (1y), " +
            "Green Tie (2y), White Tie (5y), Yellow Tie (10y), Blue Tie (15y), Pink Tie (20y).\n" +
            "- Permanent user deletion is available via `DELETE /api/users/{deviceId}` (GDPR/LGPD).",
        Contact = new OpenApiContact { Name = "Só Por Hoje", Url = contactUrl },
        License = new OpenApiLicense { Name = "MIT" },
    });

    c.SwaggerDoc("v1-ptbr", new OpenApiInfo
    {
        Title = "API Só Por Hoje",
        Version = "v1",
        Description =
            "API REST do app **Só Por Hoje** — companheiro diário de recuperação " +
            "para membros de Alcoólicos Anônimos no Brasil.\n\n" +
            "## Arquitetura\n\n" +
            "- **Runtime:** ASP.NET Core 8 Minimal API\n" +
            "- **Banco de dados:** PostgreSQL 16 + Entity Framework Core 8\n" +
            "- **Scraper:** Serviço em background que coleta reuniões online de A.A. a cada 30 minutos\n" +
            "- **Autenticação:** Anônima por UUID (sem email/senha, sem dados pessoais)\n\n" +
            "## Regras de Negócio\n\n" +
            "- O app é **offline-first**: o SQLite no dispositivo é a fonte de verdade; o servidor é uma camada de backup/sync.\n" +
            "- Todos os datetimes são UTC no protocolo; a exibição é convertida para o horário de Brasília (America/Sao_Paulo).\n" +
            "- Bitmask `DaysOfWeekMask`: Domingo=1, Segunda=2, Terça=4, Quarta=8, Quinta=16, Sexta=32, Sábado=64.\n" +
            "- As fichas de sobriedade seguem o padrão brasileiro de A.A.: Amarela (1d), Azul (90d), Rosa (180d), Vermelha (270d), " +
            "Verde (1a), Verde Gravata (2a), Branca Gravata (5a), Amarela Gravata (10a), Azul Gravata (15a), Rosa Gravata (20a).\n" +
            "- Exclusão permanente do usuário disponível via `DELETE /api/users/{deviceId}` (LGPD/GDPR).",
        Contact = new OpenApiContact { Name = "Só Por Hoje", Url = contactUrl },
        License = new OpenApiLicense { Name = "MIT" },
    });

    // Include every endpoint in both docs
    c.DocInclusionPredicate((_, _) => true);

    // Apply localized summaries, descriptions, and tag metadata
    c.DocumentFilter<LocalizedDocumentFilter>();

    // XML documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1-en/swagger.json", "Só Por Hoje API — English");
    c.SwaggerEndpoint("/swagger/v1-ptbr/swagger.json", "Só Por Hoje API — Português");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Só Por Hoje API";
});

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
