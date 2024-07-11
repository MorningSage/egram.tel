using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Utils.Reactive;

namespace Tel.Egran.ViewModels.Messaging.Informer;

public partial class InformerViewModel : AbstractViewModelBase
{
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private string _title;
    [ObservableProperty] private string _label;
    [ObservableProperty] private Avatar _avatar;
        
    public InformerViewModel(Chat chat, IAvatarLoader avatarLoader)
    {
        BindInformer(chat, avatarLoader);
    }

    public InformerViewModel(Aggregate aggregate)
    {
        BindInformer(aggregate);
    }

    private InformerViewModel() { }
    
    public static InformerViewModel Hidden() => new() { IsVisible = false };

    private void BindInformer(Chat chat, IAvatarLoader avatarLoader)
    {
        Title = chat.ChatData.Title;
        Label = chat.ChatData.Title;
                    
        avatarLoader.LoadAvatar(chat.ChatData, AvatarSize.Regular).SafeSubscribe(avatar => Avatar = avatar);
    }

    private void BindInformer(Aggregate aggregate)
    {
        Title = aggregate.Id.ToString();
        Label = aggregate.Id.ToString();
    }
}