using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary.Notifications;

public class DisplayNotificationSender : INotificationSender
{
    public void Send(INotification notification)
    {
        notification.GetMessage();
    }
}
