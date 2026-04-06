using System.Collections.ObjectModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel dos 12 Passos.</summary>
public partial class StepsViewModel : BaseViewModel
{
    public ObservableCollection<StepItem> Steps { get; }

    public StepsViewModel()
    {
        Title = "Os 12 Passos";
        Steps = new ObservableCollection<StepItem>(
            AAContent.TwelveSteps.Select((s, i) => new StepItem(i + 1, s.Title, s.Text)));
    }
}

public partial class StepItem : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    public int Number { get; }
    public string Title { get; }
    public string Text { get; }

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool _isExpanded;

    public StepItem(int number, string title, string text)
    {
        Number = number;
        Title = $"Passo {number} — {title}";
        Text = text;
    }
}
