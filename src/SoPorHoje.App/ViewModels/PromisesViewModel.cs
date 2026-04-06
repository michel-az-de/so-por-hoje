using System.Collections.ObjectModel;
using SoPorHoje.App.Constants;

namespace SoPorHoje.App.ViewModels;

/// <summary>ViewModel das Promessas do A.A.</summary>
public partial class PromisesViewModel : BaseViewModel
{
    public ObservableCollection<string> Promises { get; }

    public PromisesViewModel()
    {
        Title = "As Promessas";
        Promises = new ObservableCollection<string>(AAContent.Promises);
    }
}
