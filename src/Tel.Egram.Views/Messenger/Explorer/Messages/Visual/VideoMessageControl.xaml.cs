using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Explorer.Messages.Visual;

namespace Tel.Egram.Views.Messenger.Explorer.Messages.Visual;

public class VideoMessageControl : BaseControl<VideoMessageViewModel>
{
    public VideoMessageControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}