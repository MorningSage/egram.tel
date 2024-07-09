using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public partial class PhotoMessageModel : AbstractVisualMessageModel
{
    [ObservableProperty] private TdApi.Photo? _photoData = null;
    [ObservableProperty] private string _text = string.Empty;
}