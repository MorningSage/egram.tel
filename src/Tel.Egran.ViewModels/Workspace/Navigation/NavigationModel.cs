using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Users;

namespace Tel.Egran.ViewModels.Workspace.Navigation;

[AddINotifyPropertyChangedInterface]
public class NavigationModel : IActivatableViewModel
{
    public Avatar Avatar { get; set; }

    public int SelectedTabIndex { get; set; }

    public NavigationModel(IAvatarLoader avatarLoader, IUserLoader userLoader)
    {
        this.WhenActivated(disposables =>
        {
            this.BindUserAvatar(avatarLoader, userLoader).DisposeWith(disposables);
        });
    }
        
    public ViewModelActivator Activator { get; } = new();
}