namespace Tel.Egram.Model.Messaging.Explorer.Loaders;

public class MessageLoaderConductor
{
    public object Locker { get; } = new();
    public bool IsBusy { get; set; }
}