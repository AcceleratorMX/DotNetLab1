using ClassLibrary.Notifications.Abstract;
using ClassLibrary.Settings.Email;
using System.Net.Mail;

namespace ClassLibrary.Notifications;

public class EmailNotificationSender(EmailSettingsProvider emailSettingsProvider) : INotificationSender
{
    private readonly EmailSettings _emailSettings = emailSettingsProvider.GetEmailSettings();

    public void Send(INotification notification)
    {
        if (!notification.ShouldSendEmail())
        {
            return;
        }

        string recipientEmail = notification.GetRecipientEmail();

        if (string.IsNullOrWhiteSpace(recipientEmail))
        {
            Console.WriteLine("Notification skipped: recipient email is empty");
            return;
        }

        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail),
                Subject = "ATM Notification",
                Body = notification.GetMessage()
            };
            message.To.Add(new MailAddress(recipientEmail));

            using var client = new SmtpClient(_emailSettings.SMTPServer, _emailSettings.SMTPPort)
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(
                    _emailSettings.SMTPUsername,
                    _emailSettings.SMTPPassword
                ),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            client.Send(message);
            Console.WriteLine($"Email sent to {recipientEmail}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Invalid email format: {ex.Message}");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}
