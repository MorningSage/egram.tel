using Tel.Egram.Model.Notifications;

namespace Tel.Egram.Services.Notifications;

public interface INotificationController
{
    void Show(NotificationModel model);
}