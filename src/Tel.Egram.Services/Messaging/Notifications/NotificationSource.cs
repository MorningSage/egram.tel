using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Model.Notifications;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Notifications;

public class NotificationSource(IAgent agent) : INotificationSource
{
    /// <summary>
    /// Get notifications for new chats
    /// </summary>
    public IObservable<NotificationModel> ChatNotifications() => agent.Updates.OfType<TdApi.Update.UpdateNewChat>().Select(update => new NotificationModel
    {
        Chat = update.Chat
    });

    /// <summary>
    /// Get notifications for new messages from chats with
    /// enabled notifications and not older than 1 minute
    /// </summary>
    public IObservable<NotificationModel> MessagesNotifications() => agent.Updates
        .OfType<TdApi.Update.UpdateNewMessage>()
        // ToDo: Suppress messages sent with notification disabled - unsure if this should be via UpdateChatDefaultDisableNotification?
        .Where(u => u.Message.Date > DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60)
        .Select(update => update.Message)
        .SelectSeq(message =>
        {
            return GetChat(message.ChatId).Select(chat => new NotificationModel
            {
                Chat = chat,
                Message = message
            });
        });

    private IObservable<TdApi.Chat> GetChat(long chatId) => agent.Execute(new TdApi.GetChat
    {
        ChatId = chatId
    });
}