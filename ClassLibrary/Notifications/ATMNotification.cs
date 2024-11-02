using ClassLibrary.Notifications.Abstract;

namespace ClassLibrary.Notifications;

public class ATMNotification(string notificationMessage, string recipientEmail, bool shouldSendEmail = false) : INotification
{
    public string GetMessage() => notificationMessage;
    public string GetRecipientEmail() => recipientEmail;
    public bool ShouldSendEmail() => shouldSendEmail;
}