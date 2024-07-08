using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

[AddINotifyPropertyChangedInterface]
public class EntryViewModel : IActivatableViewModel
{
    public long Id { get; set; }
        
    public int Order { get; set; }
        
    public string Title { get; set; }
        
    public Avatar? Avatar { get; set; }

    public bool HasUnread { get; set; }

    public string UnreadCount { get; set; }

    public EntryViewModel()
    {
        this.WhenActivated(disposables =>
        {
            this.BindAvatarLoading().DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();
}