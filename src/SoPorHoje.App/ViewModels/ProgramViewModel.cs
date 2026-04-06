using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace SoPorHoje.App.ViewModels;

/// <summary>Hub de Programa — 12 Passos, Tradições, Promessas, SPH, Orações, HALT.</summary>
public partial class ProgramViewModel : BaseViewModel
{
    public ObservableCollection<ProgramSection> Sections { get; } = new()
    {
        new("📖", "Os 12 Passos",      "Programa de recuperação",              "steps"),
        new("📜", "As 12 Tradições",    "Princípios de unidade",                "traditions"),
        new("🌟", "As Promessas",       "O que esperar da recuperação",         "promises"),
        new("🌅", "Só Por Hoje",        "9 meditações diárias",                 "justfortoday"),
        new("🙏", "Orações",            "Serenidade, 3º Passo, 7º Passo",       "prayers"),
        new("🚦", "Check HALT",         "Fome, Raiva, Solidão, Cansaço",        "halt"),
    };

    public ProgramViewModel()
    {
        Title = "Programa";
    }

    [RelayCommand]
    private static async Task NavigateAsync(ProgramSection section)
    {
        await Shell.Current.GoToAsync(section.Route);
    }
}

/// <summary>Seção do hub de Programa.</summary>
public record ProgramSection(string Icon, string Title, string Description, string Route);
