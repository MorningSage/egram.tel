using System.Reactive;
using Tel.Egram.Model.Messaging.Chats;

namespace Tel.Egram.Services.Messaging.Chats;

public interface IChatUpdater
{
    IObservable<Unit> GetOrderUpdates();

    IObservable<Chat> GetChatUpdates();
}