using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Workspace.Navigation;

[AddINotifyPropertyChangedInterface]
public class NavigationModel : IActivatableViewModel
{
    public Avatar Avatar { get; set; }

    public int SelectedTabIndex { get; set; }

    public NavigationModel()
    {
        this.WhenActivated(disposables =>
        {
            this.BindUserAvatar().DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}