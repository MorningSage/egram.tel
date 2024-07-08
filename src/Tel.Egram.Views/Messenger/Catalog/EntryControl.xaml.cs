using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Messaging.Catalog.Entries;

namespace Tel.Egram.Views.Messenger.Catalog;

public class EntryControl : BaseControl<EntryViewModel>
{
    public EntryControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}