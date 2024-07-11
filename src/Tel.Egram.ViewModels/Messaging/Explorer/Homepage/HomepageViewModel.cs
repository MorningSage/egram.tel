using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData.Binding;
using Tel.Egram.Model.Messaging.Explorer.Messages;

namespace Tel.Egram.ViewModels.Messaging.Explorer.Homepage;

public partial class HomepageViewModel : AbstractViewModelBase
{
    [ObservableProperty] private bool _isVisible = true;
    [ObservableProperty] private string _searchText;
    [ObservableProperty] private ObservableCollectionExtended<MessageModel> _promotedMessages;
    
    public static HomepageViewModel Hidden()
    {
        return new HomepageViewModel
        {
            IsVisible = false
        };
    }
}