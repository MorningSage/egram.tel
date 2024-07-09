using CommunityToolkit.Mvvm.ComponentModel;

namespace Tel.Egram.Model.Popups;

public partial class PopupContext : ObservableObject
{
    [ObservableProperty] private string? _title = null;
}