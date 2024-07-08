namespace Tel.Egram.Model.Messaging.Chats;

public class Aggregate(long id, IList<Chat> chats)
{
    public long Id { get; set; } = id;
    public IList<Chat> Chats { get; } = chats;

    public Aggregate(IList<Chat> chats) : this(0, chats) { }
}