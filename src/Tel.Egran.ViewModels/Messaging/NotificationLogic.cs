using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Tel.Egram.Services;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Notifications;
using Tel.Egram.Services.Utils.Reactive;
using Tel.Egran.ViewModels.Notifications;

namespace Tel.Egran.ViewModels.Messaging;

public static class NotificationLogic
{
    private static readonly INotificationSource NotificationSource = Registry.Services.GetRequiredService<INotificationSource>();
    private static readonly INotificationController NotificationController = Registry.Services.GetRequiredService<INotificationController>();
    
    public static IDisposable BindNotifications(this MessengerViewModel viewModel)
    {
        var chats = NotificationSource.ChatNotifications();
        var messages = NotificationSource.MessagesNotifications();

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
                        NotificationController.Show(NotificationViewModel.FromNotification(notifications[0]));
                        break;
                        
                    default:
                        NotificationController.Show(NotificationViewModel.FromNotificationList(notifications));
                        break;
                }
            });
    }
}