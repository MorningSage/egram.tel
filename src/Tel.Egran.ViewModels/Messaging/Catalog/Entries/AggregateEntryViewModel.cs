using CommunityToolkit.Mvvm.ComponentModel;
using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public partial class AggregateEntryViewModel(IAvatarLoader avatarLoader) : EntryViewModel(avatarLoader)
{
    [ObservableProperty] private Aggregate _aggregate;
}