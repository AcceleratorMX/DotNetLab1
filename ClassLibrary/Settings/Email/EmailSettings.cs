namespace ClassLibrary.Settings.Email;

public class EmailSettings
{
    public string SenderEmail { get; set; } = string.Empty;
    public string SMTPServer { get; set; } = string.Empty;
    public int SMTPPort { get; set; } = 0;
    public string SMTPUsername { get; set; } = string.Empty;
    public string SMTPPassword { get; set; } = string.Empty;
}
