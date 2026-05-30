using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SoPorHoje.Core.Interfaces;

namespace SoPorHoje.App.ViewModels;

/// <summary>Tela de Marcos — todas as fichas de tempo, conquistadas e a conquistar.</summary>
public partial class MilestonesViewModel : BaseViewModel
{
    private readonly IUserRepository _users;
    private readonly IChipService _chips;

    public MilestonesViewModel(IUserRepository users, IChipService chips)
    {
        _users = users;
        _chips = chips;
        Title = "Marcos";
    }

    public ObservableCollection<MilestoneItem> Milestones { get; } = new();

    [ObservableProperty] private string _summary = string.Empty;

    public Task InitializeAsync() => RunSafeAsync(LoadAsync);

    private async Task LoadAsync()
    {
        var profile = await _users.GetProfileAsync();
        var soberDays = profile?.SoberDays ?? 0;
        var chips = _chips.GetAllChips();

        Milestones.Clear();
        foreach (var chip in chips)
        {
            var earned = soberDays >= chip.RequiredDays;
            var status = earned ? "✓ conquistado" : $"faltam {chip.RequiredDays - soberDays} dias";
            Milestones.Add(new MilestoneItem(chip.Emoji, chip.Name, chip.Label, status, earned ? 1.0 : 0.45));
        }

        Summary = $"{_chips.GetEarnedCount(soberDays)} de {chips.Count} marcos";
    }
}

/// <summary>Item da lista de Marcos.</summary>
public record MilestoneItem(string Emoji, string Name, string Period, string Status, double Opacity)
{
    public string Accessibility => $"{Name}, {Period}, {Status}";
}
