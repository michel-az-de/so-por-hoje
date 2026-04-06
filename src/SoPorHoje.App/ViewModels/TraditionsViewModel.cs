using System.Collections.ObjectModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel das 12 Tradições.</summary>
public partial class TraditionsViewModel : BaseViewModel
{
    public ObservableCollection<TraditionItem> Traditions { get; }

    public TraditionsViewModel()
    {
        Title = "As 12 Tradições";
        Traditions = new ObservableCollection<TraditionItem>(
            AAContent.TwelveTraditions.Select((t, i) => new TraditionItem(i + 1, t)));
    }
}

public partial class TraditionItem : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public int Number { get; }
    public string Title { get; }
    public string Text { get; }

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isExpanded;

    public TraditionItem(int number, string text)
    {
        Number = number;
        Title = $"Tradição {number}";
        Text = text;
    }
}
