using PropertyChanged;
using ReactiveUI;
using Tel.Egram.Model.Notifications;

namespace Tel.Egran.ViewModels.Notifications;

[AddINotifyPropertyChangedInterface]
public class NotificationViewModel : IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();

    public static NotificationModel FromNotification(NotificationModel notificationView)
    {
        var chat = notificationView.Chat;
        var message = notificationView.Message;

        if (message != null)
        {
            return new NotificationModel
            {
                Title = "New message",
                Text = chat.Title
            };
        }

        return new NotificationModel
        {
            Title = "New chat",
            Text = chat.Title
        };
    }

    public static NotificationModel FromNotificationList(IList<NotificationModel> notifications)
    {
        return new NotificationModel
        {
            Title = "New messages",
            Text = "You have new messages"
        };
    }
}