using Microsoft.Extensions.Configuration;

namespace ClassLibrary.Settings.Email;

public class EmailSettingsProvider(IConfiguration configuration)
{
    public EmailSettings GetEmailSettings()
    {
        var emailSettings = new EmailSettings();
        var emailSection = configuration.GetSection("EmailSettings");

        emailSettings.SenderEmail = emailSection["SenderEmail"] ?? string.Empty;
        emailSettings.SMTPServer = emailSection["SMTPServer"] ?? string.Empty;
        emailSettings.SMTPPort = int.Parse(emailSection["SMTPPort"] ?? "0");
        emailSettings.SMTPUsername = emailSection["SMTPUsername"] ?? string.Empty;
        emailSettings.SMTPPassword = emailSection["SMTPPassword"] ?? string.Empty;

        return emailSettings;
    }
}