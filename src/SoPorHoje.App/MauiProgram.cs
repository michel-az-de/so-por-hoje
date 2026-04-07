using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Data.Local;
using SoPorHoje.Data.Local.Repositories;
using SoPorHoje.Data.Sync;
using SoPorHoje.App.Services;
using SoPorHoje.App.ViewModels;
using SoPorHoje.App.Views;

namespace SoPorHoje.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
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

        // ── Database ──────────────────────────────────────────────────────────
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "soporhoje.db3");
        builder.Services.AddSingleton(_ => new SoPorHojeDatabase(dbPath,
            builder.Services.BuildServiceProvider()
                .GetRequiredService<ILogger<SoPorHojeDatabase>>()));

        // ── Repositories / Services ───────────────────────────────────────────
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IPledgeRepository, PledgeRepository>();
        builder.Services.AddSingleton<IReflectionRepository, ReflectionRepository>();
        builder.Services.AddSingleton<IMeetingRepository, MeetingRepository>();
        builder.Services.AddSingleton<IChipService, ChipService>();
        builder.Services.AddSingleton<ISyncService, SyncEngine>();

        // ── ViewModels ────────────────────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ChipsViewModel>();
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<SOSViewModel>();
        builder.Services.AddTransient<MeetingsViewModel>();
        builder.Services.AddTransient<ProgramViewModel>();
        builder.Services.AddTransient<StepsViewModel>();
        builder.Services.AddTransient<TraditionsViewModel>();
        builder.Services.AddTransient<PromisesViewModel>();
        builder.Services.AddTransient<JustForTodayViewModel>();
        builder.Services.AddTransient<PrayersViewModel>();
        builder.Services.AddTransient<HaltCheckViewModel>();

        // ── Views ─────────────────────────────────────────────────────────────
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ChipsPage>();
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<SOSPage>();
        builder.Services.AddTransient<MeetingsPage>();
        builder.Services.AddTransient<ProgramPage>();
        builder.Services.AddTransient<StepsPage>();
        builder.Services.AddTransient<TraditionsPage>();
        builder.Services.AddTransient<PromisesPage>();
        builder.Services.AddTransient<JustForTodayPage>();
        builder.Services.AddTransient<PrayersPage>();
        builder.Services.AddTransient<HaltCheckPage>();

        // ── Shell ─────────────────────────────────────────────────────────────
        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
