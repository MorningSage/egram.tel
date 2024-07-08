using System.Reactive.Disposables;
using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Avatars;

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

    public EntryViewModel(IAvatarLoader avatarLoader)
    {
        this.WhenActivated(disposables =>
        {
            this.BindAvatarLoading(avatarLoader).DisposeWith(disposables);
        });
    }

    public ViewModelActivator Activator { get; } = new();
}