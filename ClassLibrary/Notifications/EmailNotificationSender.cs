using System.Net;
using System.Net.Mail;
using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;

namespace ClassLibrary.Notifications;

public class EmailNotificationSender : INotificationSender
{
    private readonly EmailSettings _emailSettings;
    private readonly Dictionary<Type, Action<Exception>> _exceptionHandlers;

    public EmailNotificationSender(EmailSettingsProvider emailSettingsProvider)
    {
        _emailSettings = emailSettingsProvider.GetEmailSettings();

        _exceptionHandlers = new Dictionary<Type, Action<Exception>>
        {
            { typeof(FormatException), ex => Console.WriteLine($"Invalid email format: {ex.Message}") },
            { typeof(SmtpException), ex => Console.WriteLine($"SMTP error: {ex.Message}") },
            { typeof(Exception), ex => Console.WriteLine($"Failed to send email: {ex.Message}") }
        };
    }

    public void Send(INotification notification)
    {
        if (!notification.ShouldSendEmail() || !ValidateRecipientEmail(notification, out var recipientEmail))
        {
            return;
        }

        try
        {
            using var message = CreateMailMessage(notification, recipientEmail);
            using var client = CreateSmtpClient();
            client.Send(message);
            Console.WriteLine($"Email sent to {recipientEmail}");
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    private static bool ValidateRecipientEmail(INotification notification, out string recipientEmail)
    {
        recipientEmail = notification.GetRecipientEmail();
        if (!string.IsNullOrWhiteSpace(recipientEmail)) return true;
        Console.WriteLine("Notification skipped: recipient email is empty");
        return false;
    }

    private MailMessage CreateMailMessage(INotification notification, string recipientEmail)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail),
            Subject = "ATM Notification",
            Body = notification.GetMessage()
        };
        message.To.Add(new MailAddress(recipientEmail));
        return message;
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_emailSettings.SMTPServer, _emailSettings.SMTPPort)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(
                _emailSettings.SMTPUsername,
                _emailSettings.SMTPPassword
            ),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };
    }

    private void HandleException(Exception ex)
    {
        if (_exceptionHandlers.TryGetValue(ex.GetType(), out var handler))
        {
            handler(ex);
        }
        else
        {
            _exceptionHandlers[typeof(Exception)](ex);
        }
    }
}