using Application.DTOs.Shared;

namespace Application.Interfaces.Shared;

public interface IMailService
{
    Task SendEmailAsync(MailRequest request, CancellationToken ct = default);
}
