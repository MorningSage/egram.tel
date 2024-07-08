using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

namespace Tel.Egram.Views.Messenger.Explorer.Messages.Visual;

public class StickerMessageControl : BaseControl<StickerMessageViewModel>
{
    public StickerMessageControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}