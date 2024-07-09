using CommunityToolkit.Mvvm.ComponentModel;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Basic;

public partial class BasicMessageModel : MessageModel
{
    [ObservableProperty] private string _text = string.Empty;
}