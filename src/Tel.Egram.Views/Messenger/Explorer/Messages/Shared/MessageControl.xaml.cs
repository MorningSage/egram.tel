using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Tel.Egram.Views.Messenger.Explorer.Messages.Shared;

public class MessageControl : ContentControl
{
    public MessageControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}