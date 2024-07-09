using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public partial class VideoMessageModel : AbstractVisualMessageModel
{
    [ObservableProperty] private TdApi.Video? _videoData = null;
    [ObservableProperty] private string _caption = string.Empty;
}