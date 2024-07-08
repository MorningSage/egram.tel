using Tel.Egram.Services.Graphics.Avatars;

namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public class HomeEntryViewModel : EntryViewModel
{
    private static HomeEntryViewModel? _instance;
    private HomeEntryViewModel(IAvatarLoader avatarLoader) : base(avatarLoader) { }

    public static HomeEntryViewModel GetInstance(IAvatarLoader avatarLoader) => _instance ??= new HomeEntryViewModel(avatarLoader);

    //public static HomeEntryViewModel Instance { get; } = new()
    //{
    //    Id    = -1,
    //    Order = -1,
    //    Title = "Home"
    //};
}