using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Services;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Workspace.Navigation;

public static class UserAvatarLogic
{
    private static readonly IAvatarLoader AvatarLoader = Registry.Services.GetRequiredService<IAvatarLoader>();
    private static readonly IUserLoader UserLoader = Registry.Services.GetRequiredService<IUserLoader>();
    
    public static IDisposable BindUserAvatar(this NavigationModel model)
    {
        return UserLoader
            .GetMe()
            .SelectMany(user => AvatarLoader.LoadAvatar(user.UserData, AvatarSize.Regular))
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(avatar => { model.Avatar = avatar; });
    }
}