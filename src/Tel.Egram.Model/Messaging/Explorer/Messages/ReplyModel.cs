using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;
using Tel.Egram.Model.Messaging.Explorer.Messages.Visual;
using Tel.Egram.Model.Messaging.Messages;

namespace Tel.Egram.Model.Messaging.Explorer.Messages;

public partial class ReplyModel : ObservableObject
{
    [ObservableProperty] private string _authorName = string.Empty;
    [ObservableProperty] private string? _text = null;
    
    // ToDo: Can this be automatic based on Preview?
    [ObservableProperty] private bool _hasPreview = false;
    [ObservableProperty] private Preview? _preview = null;
    [ObservableProperty] private Message? _message = null;
    [ObservableProperty] private TdApi.Photo? _photoData = null;
    [ObservableProperty] private TdApi.Sticker? _stickerData = null;
    [ObservableProperty] private TdApi.Video? _videoData = null;
}