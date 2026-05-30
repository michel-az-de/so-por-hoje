using System.Collections.ObjectModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel da Trilha — princípios de recuperação.</summary>
public partial class StepsViewModel : BaseViewModel
{
    public ObservableCollection<StepItem> Steps { get; }

    public StepsViewModel()
    {
        Title = "A Trilha";
        Steps = new ObservableCollection<StepItem>(
            ProgramContent.Trilha.Select((s, i) => new StepItem(i + 1, s.Title, s.Text)));
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
        Title = $"{number}. {title}";
        Text = text;
    }
}
