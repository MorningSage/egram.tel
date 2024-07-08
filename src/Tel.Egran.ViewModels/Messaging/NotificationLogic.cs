using System.Reactive.Linq;
using ReactiveUI;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Notifications;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Notifications;

namespace Tel.Egran.ViewModels.Messaging;

public static class NotificationLogic
{
    //private static readonly INotificationSource NotificationSource = Registry.Services.GetRequiredService<INotificationSource>();
    //private static readonly INotificationController NotificationController = Registry.Services.GetRequiredService<INotificationController>();
    
    public static IDisposable BindNotifications(this MessengerViewModel viewModel, INotificationSource notificationSource, INotificationController notificationController)
    {
        var chats = notificationSource.ChatNotifications();
        var messages = notificationSource.MessagesNotifications();

        return messages.Merge(chats)
            .Buffer(TimeSpan.FromSeconds(2))
            .SubscribeOn(RxApp.TaskpoolScheduler)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Accept(notifications =>
            {
                switch (notifications.Count)
                {
                    case 0:
                        break;
                        
                    case 1:
                        notificationController.Show(NotificationViewModel.FromNotification(notifications[0]));
                        break;
                        
                    default:
                        notificationController.Show(NotificationViewModel.FromNotificationList(notifications));
                        break;
                }
            });
    }
}