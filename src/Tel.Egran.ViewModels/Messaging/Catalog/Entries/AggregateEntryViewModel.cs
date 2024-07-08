using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public class AggregateEntryViewModel(IAvatarLoader avatarLoader) : EntryViewModel(avatarLoader)
{
    public Aggregate Aggregate { get; set; }
}