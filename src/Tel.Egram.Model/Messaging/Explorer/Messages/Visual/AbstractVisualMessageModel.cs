using CommunityToolkit.Mvvm.ComponentModel;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Visual;

public abstract partial class AbstractVisualMessageModel : MessageModel
{
    [ObservableProperty] private Preview? _preview = null;
}