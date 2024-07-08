using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Explorer.Messages.Basic;

namespace Tel.Egram.Views.Messenger.Explorer.Messages.Basic;

public class TextMessageControl : BaseControl<TextMessageViewModel>
{
    public TextMessageControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}