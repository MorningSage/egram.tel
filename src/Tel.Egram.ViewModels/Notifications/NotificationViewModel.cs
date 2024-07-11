using Tel.Egram.Model.Notifications;

namespace Tel.Egram.ViewModels.Notifications;

public class NotificationViewModel : AbstractViewModelBase
{
    public static NotificationModel FromNotification(NotificationModel notificationView) => notificationView.Message switch
    {
        null => new NotificationModel { Title = "New chat",    Text = notificationView.Chat.Title },
        _    => new NotificationModel { Title = "New message", Text = notificationView.Chat.Title }
    };

    public static NotificationModel FromNotificationList(IList<NotificationModel> _) => new()
    {
        Title = "New messages",
        Text  = "You have new messages"
    };
}