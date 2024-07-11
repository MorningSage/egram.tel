using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egram.ViewModels.Workspace.Navigation;

public partial class NavigationModel : AbstractViewModelBase
{
    [ObservableProperty] private Avatar? _avatar;
    [ObservableProperty] private int _selectedTabIndex;

    public NavigationModel(IAvatarLoader avatarLoader, IUserLoader userLoader)
    {
        userLoader.GetMe()
            .SelectMany(user => avatarLoader.LoadAvatar(user.UserData, AvatarSize.Regular))
            .SafeSubscribe(avatar => Avatar = avatar);
    }
}