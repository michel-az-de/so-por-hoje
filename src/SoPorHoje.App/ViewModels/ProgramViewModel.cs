using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;

namespace SoPorHoje.App.ViewModels;

/// <summary>Hub de Programa — A Trilha, Combinados, As Promessas, Só Por Hoje, Meditações, HALT.</summary>
public partial class ProgramViewModel : BaseViewModel
{
    public ObservableCollection<ProgramSection> Sections { get; } = new()
    {
        new("📖", "A Trilha",     "Princípios para a recuperação",         "steps"),
        new("🤝", "Combinados",   "Acordos comigo mesmo, só por hoje",     "traditions"),
        new("🌟", "As Promessas", "O que a recuperação devolve",           "promises"),
        new("🌅", "Só Por Hoje",  "Viver um dia de cada vez",              "justfortoday"),
        new("🧘", "Meditações",   "Pausas para respirar e se reencontrar", "prayers"),
        new("🚦", "Check HALT",   "Fome, Raiva, Solidão, Cansaço",         "halt"),
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
