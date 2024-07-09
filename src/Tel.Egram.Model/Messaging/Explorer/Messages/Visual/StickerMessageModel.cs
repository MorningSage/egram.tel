using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public partial class StickerMessageModel : AbstractVisualMessageModel
{
    [ObservableProperty] private TdApi.Sticker? _stickerData = null;
}