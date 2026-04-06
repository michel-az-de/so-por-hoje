using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SoPorHoje.App.Converters;
using SoPorHoje.App.Interfaces;
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

        // ── Services ──────────────────────────────────────────────────────────
        builder.Services.AddSingleton<IMeetingRepository, InMemoryMeetingRepository>();

        // ── ViewModels ────────────────────────────────────────────────────────
        builder.Services.AddTransient<MeetingsViewModel>();
        builder.Services.AddTransient<ProgramViewModel>();
        builder.Services.AddTransient<StepsViewModel>();
        builder.Services.AddTransient<TraditionsViewModel>();
        builder.Services.AddTransient<PromisesViewModel>();
        builder.Services.AddTransient<JustForTodayViewModel>();
        builder.Services.AddTransient<PrayersViewModel>();
        builder.Services.AddTransient<HaltCheckViewModel>();

        // ── Views ─────────────────────────────────────────────────────────────
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
