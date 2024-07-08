using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Informer;

namespace Tel.Egram.Views.Messenger.Informer;

public class InformerControl : BaseControl<InformerViewModel>
{
    public InformerControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}