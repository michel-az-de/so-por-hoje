using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel do Check HALT com cards interativos expansíveis.</summary>
public partial class HaltCheckViewModel : BaseViewModel
{
    public ObservableCollection<HaltItem> Items { get; }

    public HaltCheckViewModel()
    {
        Title = "Check HALT";
        Items = new ObservableCollection<HaltItem>(
            AAContent.HaltCheck.Select(h => new HaltItem(h.Letter, h.Word, h.Question, h.Tip, h.Emoji)));
    }
}

/// <summary>Item HALT com estado de expansão e cor associada.</summary>
public partial class HaltItem : ObservableObject
{
    public string Letter { get; }
    public string Word { get; }
    public string Question { get; }
    public string Tip { get; }
    public string Emoji { get; }
    public Color CardColor { get; }

    [ObservableProperty]
    private bool _isExpanded;

    private static readonly Color[] _colors =
    {
        Color.FromArgb("#FF6B35"),  // H - Fome
        Color.FromArgb("#E74C3C"),  // A - Raiva
        Color.FromArgb("#8E44AD"),  // L - Solidão
        Color.FromArgb("#2980B9"),  // T - Cansaço
    };

    public HaltItem(string letter, string word, string question, string tip, string emoji)
    {
        Letter = letter;
        Word = word;
        Question = question;
        Tip = tip;
        Emoji = emoji;
        CardColor = letter switch
        {
            "H" => _colors[0],
            "A" => _colors[1],
            "L" => _colors[2],
            "T" => _colors[3],
            _   => Colors.Gray,
        };
    }

    public string AccessibilityDescription =>
        IsExpanded
            ? $"{Word}. {Question} Toque para recolher."
            : $"{Word}. {Question} Toque para ver dicas.";
}
