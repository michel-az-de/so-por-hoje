using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.Core.Interfaces;
using SoPorHoje.Core.Models;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel da tela de Fichas com sistema de celebração de conquistas.</summary>
public partial class ChipsViewModel : BaseViewModel
{
    private readonly IChipService _chipService;
    private readonly IUserRepository _userRepo;

    [ObservableProperty]
    private ObservableCollection<ChipDisplayItem> _chips = new();

    [ObservableProperty]
    private int _earnedCount;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private string _progressText = string.Empty;

    [ObservableProperty]
    private bool _showCelebration;

    [ObservableProperty]
    private ChipDisplayItem? _celebrationChip;

    public ChipsViewModel(IChipService chipService, IUserRepository userRepo)
    {
        _chipService = chipService;
        _userRepo = userRepo;
        Title = "Fichas";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            var profile = await _userRepo.GetProfileAsync();
            var soberDays = profile?.SoberDays ?? 0;

            var allChips = _chipService.GetAllChips();
            TotalCount = allChips.Count;
            EarnedCount = _chipService.GetEarnedCount(soberDays);
            ProgressText = $"{EarnedCount} de {TotalCount} conquistadas";

            var items = allChips.Select(c => new ChipDisplayItem(c, soberDays)).ToList();
            Chips = new ObservableCollection<ChipDisplayItem>(items);

            // Check for uncelebrated chips
            var uncelebrated = await _chipService.GetUncelebratedAsync(soberDays);
            if (uncelebrated.Count > 0)
            {
                var chip = allChips.FirstOrDefault(c => c.RequiredDays == uncelebrated[0].ChipRequiredDays);
                if (chip is not null)
                {
                    CelebrationChip = new ChipDisplayItem(chip, soberDays);
                    ShowCelebration = true;
                }
            }
        });
    }

    [RelayCommand]
    private async Task DismissCelebrationAsync()
    {
        if (CelebrationChip is null) return;
        await _chipService.MarkCelebratedAsync(CelebrationChip.Chip.RequiredDays);
        ShowCelebration = false;
        CelebrationChip = null;
    }
}

/// <summary>Item de exibição de ficha com estado de progresso calculado.</summary>
public partial class ChipDisplayItem : ObservableObject
{
    public SobrietyChip Chip { get; }
    public bool IsEarned { get; }
    public int DaysUntil { get; }
    public string DaysUntilText { get; }
    public double Opacity => IsEarned ? 1.0 : 0.3;

    public ChipDisplayItem(SobrietyChip chip, int soberDays)
    {
        Chip = chip;
        IsEarned = chip.IsEarned(soberDays);
        DaysUntil = IsEarned ? 0 : chip.RequiredDays - soberDays;
        DaysUntilText = IsEarned ? "✓ Conquistada" : $"Faltam {DaysUntil} dias";
    }
}
