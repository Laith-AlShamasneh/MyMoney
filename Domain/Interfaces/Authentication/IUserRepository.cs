using Domain.Entities.Authentication;

namespace Domain.Interfaces.Authentication;

public interface IUserRepository
{
    Task<RegisterLoginResultVM> Register(RegisterVM register);
    Task<RegisterLoginResultVM?> Login(LoginVM register);
    Task UpdateProfile(RegisterVM register, long userId);
    Task UpdatePassword(long userId, string newPassword);
    Task SetPasswordResetToken(long userId, string tokenHash, DateTime expiry);
    Task ResetPassword(long userId, string newPasswordHash);
    Task ClearResetToken(long userId);
}
