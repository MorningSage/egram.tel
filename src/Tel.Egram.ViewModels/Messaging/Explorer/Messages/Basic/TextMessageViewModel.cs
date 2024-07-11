using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Messaging.Explorer.Messages;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Messages.Basic;

public partial class TextMessageViewModel : AbstractMessageViewModel<MessageModel>
{
    [ObservableProperty] private string _text;
    
    public TextMessageViewModel(IAvatarLoader avatarLoader, IPreviewLoader previewLoader) : base(avatarLoader, previewLoader)
    {

    }
}