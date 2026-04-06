using System.Collections.ObjectModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel das meditações Só Por Hoje.</summary>
public partial class JustForTodayViewModel : BaseViewModel
{
    public ObservableCollection<string> Meditations { get; }

    public JustForTodayViewModel()
    {
        Title = "Só Por Hoje";
        Meditations = new ObservableCollection<string>(AAContent.JustForToday);
    }
}
