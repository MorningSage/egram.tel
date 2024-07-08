using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Workspace.Navigation;

public static class UserAvatarLogic
{
    public static IDisposable BindUserAvatar(this NavigationModel model, IAvatarLoader avatarLoader, IUserLoader userLoader)
    {
        return userLoader
            .GetMe()
            .SelectMany(user => avatarLoader.LoadAvatar(user.UserData, AvatarSize.Regular))
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(avatar => { model.Avatar = avatar; });
    }
}