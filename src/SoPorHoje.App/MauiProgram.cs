using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SoPorHoje.App.Converters;
using SoPorHoje.App.Interfaces;
using SoPorHoje.App.Services;
using SoPorHoje.App.ViewModels;
using SoPorHoje.App.Views;
using SoPorHoje.Data.Local;
using SoPorHoje.Data.Local.Repositories;

namespace SoPorHoje.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Garante o provedor nativo do SQLite registrado antes de qualquer acesso ao banco
        // (importante em builds Android/Release com linker agressivo).
        SQLitePCL.Batteries_V2.Init();

        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("DMSans-Regular.ttf",      "DMSans");
                fonts.AddFont("DMSans-Bold.ttf",         "DMSansBold");
                fonts.AddFont("SourceSerif4-Regular.ttf", "SourceSerif4");
                fonts.AddFont("SourceSerif4-Italic.ttf",  "SourceSerif4Italic");
            });

        // ── Services ──────────────────────────────────────────────────────────
        builder.Services.AddSingleton<IMeetingRepository, InMemoryMeetingRepository>();

        // ── Dados offline-first (SQLite) ──────────────────────────────────────
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "soporhoje.db3");
        builder.Services.AddSingleton(sp =>
            new SoPorHojeDatabase(dbPath, sp.GetRequiredService<ILogger<SoPorHojeDatabase>>()));
        builder.Services.AddSingleton<SoPorHoje.Core.Interfaces.IUserRepository, UserRepository>();
        builder.Services.AddSingleton<SoPorHoje.Core.Interfaces.IPledgeRepository, PledgeRepository>();
        builder.Services.AddSingleton<SoPorHoje.Core.Interfaces.IReflectionRepository, ReflectionRepository>();
        builder.Services.AddSingleton<SoPorHoje.Core.Interfaces.IChipService, ChipService>();

        // ── ViewModels ────────────────────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<MeetingsViewModel>();
        builder.Services.AddTransient<ProgramViewModel>();
        builder.Services.AddTransient<StepsViewModel>();
        builder.Services.AddTransient<TraditionsViewModel>();
        builder.Services.AddTransient<PromisesViewModel>();
        builder.Services.AddTransient<JustForTodayViewModel>();
        builder.Services.AddTransient<PrayersViewModel>();
        builder.Services.AddTransient<HaltCheckViewModel>();
        builder.Services.AddTransient<MilestonesViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();

        // ── Views ─────────────────────────────────────────────────────────────
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MeetingsPage>();
        builder.Services.AddTransient<ProgramPage>();
        builder.Services.AddTransient<StepsPage>();
        builder.Services.AddTransient<TraditionsPage>();
        builder.Services.AddTransient<PromisesPage>();
        builder.Services.AddTransient<JustForTodayPage>();
        builder.Services.AddTransient<PrayersPage>();
        builder.Services.AddTransient<HaltCheckPage>();
        builder.Services.AddTransient<MilestonesPage>();
        builder.Services.AddTransient<HistoryPage>();

        #if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
