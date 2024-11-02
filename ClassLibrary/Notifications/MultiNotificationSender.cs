using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary.Notifications;

public class MultiNotificationSender(params INotificationSender[] senders) : INotificationSender
{
    private readonly List<INotificationSender> _senders = new(senders);

    public void Send(INotification notification)
    {
        foreach (var sender in _senders)
        {
            sender.Send(notification);
        }
    }
}
