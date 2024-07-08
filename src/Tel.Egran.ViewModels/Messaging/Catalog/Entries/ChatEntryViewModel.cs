using Tel.Egram.Model.Messaging.Chats;
using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public class ChatEntryViewModel(IAvatarLoader avatarLoader) : EntryViewModel(avatarLoader)
{
    public Chat Chat { get; set; }
}