using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SoPorHoje.Core.Interfaces;

namespace SoPorHoje.App.ViewModels;

/// <summary>Tela de Histórico — combinados cumpridos e estatísticas pessoais.</summary>
public partial class HistoryViewModel : BaseViewModel
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    private readonly IUserRepository _users;
    private readonly IPledgeRepository _pledges;

    public HistoryViewModel(IUserRepository users, IPledgeRepository pledges)
    {
        _users = users;
        _pledges = pledges;
        Title = "Histórico";
    }

    public ObservableCollection<PledgeHistoryItem> Pledges { get; } = new();

    [ObservableProperty] private string _soberDaysText = string.Empty;
    [ObservableProperty] private string _streakText = string.Empty;
    [ObservableProperty] private string _totalText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _hasPledges;

    public bool IsEmpty => !HasPledges;

    public Task InitializeAsync() => RunSafeAsync(LoadAsync);

    private async Task LoadAsync()
    {
        var profile = await _users.GetProfileAsync();
        var soberDays = profile?.SoberDays ?? 0;
        SoberDaysText = soberDays == 1 ? "1 dia" : $"{soberDays} dias";

        var streak = await _pledges.GetStreakAsync();
        StreakText = streak == 1 ? "1 dia" : $"{streak} dias";

        var total = await _pledges.GetTotalPledgesAsync();
        TotalText = total == 1 ? "1 combinado" : $"{total} combinados";

        var history = await _pledges.GetHistoryAsync(90);
        Pledges.Clear();
        foreach (var p in history)
        {
            var label = Capitalize(p.PledgeDate.ToString("dddd, dd 'de' MMMM", PtBr));
            Pledges.Add(new PledgeHistoryItem(label));
        }

        HasPledges = Pledges.Count > 0;
    }

    private static string Capitalize(string s) =>
        string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0], PtBr) + s[1..];
}

/// <summary>Item do histórico de combinados.</summary>
public record PledgeHistoryItem(string Label);
