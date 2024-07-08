using DynamicData.Binding;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Messaging.Explorer.Messages;

namespace Tel.Egran.ViewModels.Messaging.Explorer.Homepage;

[AddINotifyPropertyChangedInterface]
public class HomepageViewModel : IActivatableViewModel
{
    public bool IsVisible { get; set; } = true;
        
    public string SearchText { get; set; }
        
    public ObservableCollectionExtended<MessageModel> PromotedMessages { get; set; }
    
    public ViewModelActivator Activator => new();

    public static HomepageViewModel Hidden()
    {
        return new HomepageViewModel
        {
            IsVisible = false
        };
    }
}