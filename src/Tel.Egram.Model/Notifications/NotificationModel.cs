using TdLib;

namespace Tel.Egram.Model.Notifications;

public class NotificationModel
{
    public TdApi.Chat Chat { get; init; }
    public TdApi.Message? Message { get; init; }
    
    public string Title { get; set; }
    public string Text { get; set; }
}