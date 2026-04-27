using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Shared;

public class MailRequest
{
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public IList<IFormFile>? Attachments { get; set; }

    // Constructor for simple emails
    public MailRequest(string toEmail, string subject, string body)
    {
        ToEmail = toEmail;
        Subject = subject;
        Body = body;
    }

    // Constructor for emails with attachments
    public MailRequest() { }
}
