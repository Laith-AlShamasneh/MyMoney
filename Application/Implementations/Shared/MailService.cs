using Application.DTOs.Shared;
using Application.Helpers;
using Application.Interfaces.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Application.Implementations.Shared;

public class MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger) : IMailService
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<MailService> _logger = logger;

    public async Task SendEmailAsync(MailRequest request, CancellationToken ct = default)
    {
        try
        {
            using var smtp = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                Credentials = new NetworkCredential(_mailSettings.FromEmail, _mailSettings.Password),
                EnableSsl = _mailSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var email = new MailMessage
            {
                From = new MailAddress(_mailSettings.FromEmail, _mailSettings.DisplayName),
                Subject = request.Subject,
                Body = request.Body,
                IsBodyHtml = true
            };

            email.To.Add(request.ToEmail);

            if (request.Attachments != null && request.Attachments.Any())
            {
                foreach (var file in request.Attachments)
                {
                    if (file.Length > 0)
                    {
                        var stream = file.OpenReadStream();
                        var attachment = new Attachment(stream, file.FileName);
                        email.Attachments.Add(attachment);
                    }
                }
            }

            await smtp.SendMailAsync(email, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", request.ToEmail);
            throw;
        }
    }
}