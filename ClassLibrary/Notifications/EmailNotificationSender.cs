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

        if (string.IsNullOrWhiteSpace(recipientEmail) || !IsValidEmail(recipientEmail))
        {
            Console.WriteLine("Помилка: Некоректний email отримувача або він порожній.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) ||
            string.IsNullOrWhiteSpace(_emailSettings.SMTPServer) ||
            _emailSettings.SMTPPort <= 0)
        {
            Console.WriteLine("Помилка: Некоректні налаштування SMTP.");
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
            Console.WriteLine($"Email надіслано до {recipientEmail}");
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SMTP помилка: {ex.Message}");
        }
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

}
