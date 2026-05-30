using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel das Meditações com cópia para a área de transferência.</summary>
public partial class PrayersViewModel : BaseViewModel
{
    public ObservableCollection<PrayerItem> Prayers { get; }

    public PrayersViewModel()
    {
        Title = "Meditações";
        Prayers = new ObservableCollection<PrayerItem>(
            ProgramContent.Meditacoes.Select(p => new PrayerItem(p.Name, p.Text)));
    }

    [RelayCommand]
    private static async Task CopyPrayerAsync(PrayerItem prayer)
    {
        await Clipboard.SetTextAsync(prayer.Text);

        if (Application.Current?.MainPage is not null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Copiado ✓",
                $"\"{prayer.Name}\" copiada para a área de transferência.",
                "OK");
        }
    }
}

public class PrayerItem
{
    public string Name { get; }
    public string Text { get; }

    public PrayerItem(string name, string text)
    {
        Name = name;
        Text = text;
    }
}
