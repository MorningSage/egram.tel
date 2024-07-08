using Tel.Egram.Model.Notifications;

namespace Tel.Egram.Services.Messaging.Notifications;

public interface INotificationSource
{
    IObservable<NotificationModel> ChatNotifications();

    IObservable<NotificationModel> MessagesNotifications();
}