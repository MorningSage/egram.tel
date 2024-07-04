using Avalonia.Markup.Xaml;
using Tel.Egram.Model.Messenger.Homepage;

namespace Tel.Egram.Views.Messenger.Homepage;

public class HomepageControl : BaseControl<HomepageModel>
{   
    public HomepageControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}