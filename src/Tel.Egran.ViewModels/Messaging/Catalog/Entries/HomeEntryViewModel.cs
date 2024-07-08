namespace Tel.Egran.ViewModels.Messaging.Catalog.Entries;

public class HomeEntryViewModel : EntryViewModel
{
    private HomeEntryViewModel() { }
    
    public static HomeEntryViewModel Instance { get; } = new()
    {
        Id    = -1,
        Order = -1,
        Title = "Home"
    };
}