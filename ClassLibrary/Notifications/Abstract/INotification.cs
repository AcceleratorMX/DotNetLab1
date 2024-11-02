namespace ClassLibrary.Notifications.Abstract;

public interface INotification
{
    string GetMessage();
    string GetRecipientEmail();
    bool ShouldSendEmail();
}
