using System.Reactive.Subjects;
using Tel.Egram.Model.Notifications;

namespace Tel.Egram.Services.Notifications;

public class NotificationController : INotificationController
{
    public Subject<NotificationModel> Trigger { get; } = new();
        
    public void Show(NotificationModel model)
    {
        Trigger.OnNext(model);
    }
}