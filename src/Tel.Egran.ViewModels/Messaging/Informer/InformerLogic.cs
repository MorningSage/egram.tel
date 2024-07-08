using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Informer;

public static class InformerLogic
{
    public static IDisposable BindInformer(this InformerViewModel viewModel, Chat chat, IAvatarLoader avatarLoader)
    {
        viewModel.Title = chat.ChatData.Title;
        viewModel.Label = chat.ChatData.Title;
                    
        return avatarLoader.LoadAvatar(chat.ChatData, AvatarSize.Regular)
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(avatar => viewModel.Avatar = avatar);
    }

    public static IDisposable BindInformer(this InformerViewModel viewModel, Aggregate aggregate)
    {
        viewModel.Title = aggregate.Id.ToString();
        viewModel.Label = aggregate.Id.ToString();
            
        return Disposable.Empty;
    }
}