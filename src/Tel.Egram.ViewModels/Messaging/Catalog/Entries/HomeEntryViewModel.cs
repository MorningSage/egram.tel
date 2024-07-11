using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egram.ViewModels.Messaging.Catalog.Entries;

public class HomeEntryViewModel : EntryViewModel
{
    private static HomeEntryViewModel? _instance;
    
    private HomeEntryViewModel(IAvatarLoader avatarLoader) : base(avatarLoader) { }

    public static HomeEntryViewModel GetInstance(IAvatarLoader avatarLoader) => _instance ??= new HomeEntryViewModel(avatarLoader);
}