namespace Application.Helpers;

public class MailSettings
{
    public string DisplayName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool EnableSsl { get; set; }
}
