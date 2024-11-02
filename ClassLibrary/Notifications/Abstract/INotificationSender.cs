namespace ClassLibrary.Notifications.Abstract;

public interface INotificationSender
{
    void Send(INotification notification);
}