using SoPorHoje.App.Views;

namespace SoPorHoje.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        Routing.RegisterRoute("steps",       typeof(StepsPage));
        Routing.RegisterRoute("traditions",  typeof(TraditionsPage));
        Routing.RegisterRoute("promises",    typeof(PromisesPage));
        Routing.RegisterRoute("justfortoday", typeof(JustForTodayPage));
        Routing.RegisterRoute("prayers",     typeof(PrayersPage));
        Routing.RegisterRoute("halt",        typeof(HaltCheckPage));
    }
}
