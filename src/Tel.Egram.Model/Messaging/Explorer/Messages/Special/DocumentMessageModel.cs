using CommunityToolkit.Mvvm.ComponentModel;
using TdLib;

namespace Tel.Egram.Model.Messaging.Explorer.Messages.Special;

public partial class DocumentMessageModel : AbstractSpecialMessageModel
{
    [ObservableProperty] private TdApi.Document? _document = null;
    [ObservableProperty] private string _fileName = string.Empty;
    [ObservableProperty] private string _caption = string.Empty;
    [ObservableProperty] private string _size = string.Empty;
}