using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egran.ViewModels.Messaging;
using Tel.Egran.ViewModels.Settings;
using Tel.Egran.ViewModels.Workspace.Navigation;

namespace Tel.Egran.ViewModels.Workspace;

[AddINotifyPropertyChangedInterface]
public class WorkspaceModel : IActivatableViewModel
{
    public NavigationModel NavigationModel { get; set; }
        
    public MessengerViewModel? MessengerModel { get; set; }
        
    public SettingsModel? SettingsModel { get; set; }
        
    public int ContentIndex { get; set; }

    public WorkspaceModel()
    {
        this.WhenActivated(disposables =>
        {
            this.BindNavigation().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}