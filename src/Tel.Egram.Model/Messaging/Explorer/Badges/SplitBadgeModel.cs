using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Messaging.Explorer.Items;

namespace Tel.Egram.Model.Messaging.Explorer.Badges;

public partial class SplitBadgeModel : ItemModel
{
    [ObservableProperty] private string _text = string.Empty;
}