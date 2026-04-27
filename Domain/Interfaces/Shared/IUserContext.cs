using Domain.Shared;

namespace Domain.Interfaces.Shared;

public interface IUserContext
{
    long UserId { get; }
    long PersonId { get; }
    Languages Language { get; }
    bool IsAuthenticated { get; }
    string IpAddress { get; }
    string UserAgent { get; }
}
