using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Editor;

namespace Tel.Egram.Views.Messenger.Editor;

public class EditorControl : BaseControl<EditorViewModel>
{
    public EditorControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}