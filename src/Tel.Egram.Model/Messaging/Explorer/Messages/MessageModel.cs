using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Model.Messaging.Explorer.Items;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Model.Messaging.Explorer.Messages;

public abstract partial class MessageModel : ItemModel
{
    [ObservableProperty] private string _authorName = string.Empty;
    [ObservableProperty] private string _time = string.Empty;
    [ObservableProperty] private Avatar? _avatar = null;
    [ObservableProperty] private Message? _message = null;
    
    // ToDo: Can this reference "Reply" and be automatic?  Why is this separate?
    [ObservableProperty] private bool _hasReply = false;
    
    [ObservableProperty] private ReplyModel? _reply = null;
}