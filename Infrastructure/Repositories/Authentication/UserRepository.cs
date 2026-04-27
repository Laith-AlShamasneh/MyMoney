using Domain.Entities.Authentication;
using Domain.Interfaces.Authentication;
using Domain.Interfaces.Shared;

namespace Infrastructure.Repositories.Authentication;

internal class UserRepository(IUnitOfWork uow) : IUserRepository
{
    private readonly IUnitOfWork _uow = uow;

    public async Task<RegisterLoginResultVM> Register(RegisterVM register)
    {
        var parameters = new
        {
            register.Username,
            register.Email,
            register.PasswordHash,
            register.FullEnglishName,
            register.FullArabicName,
            register.BirthDate,
            register.PhoneNumber,
            register.Address,
            register.CountryId,
            register.CityId,
            register.ProfilePicture,
            RoleId = (int)register.UserType,
            register.LanguageId
        };

        var result = await _uow.GlobalActions.ExecuteSingleAsync<RegisterLoginResultVM>(
            "usp_Auth_Register",
            parameters);

        return result!;
    }

    public async Task<RegisterLoginResultVM?> Login(LoginVM login)
    {
        return await _uow.GlobalActions.ExecuteSingleAsync<RegisterLoginResultVM>(
            "usp_Auth_Login",
            login);
    }

    public async Task UpdateProfile(RegisterVM register, long userId)
    {
        var parameters = new
        {
            UserId = userId,
            register.Username,
            register.Email,
            register.PasswordHash,
            register.FullEnglishName,
            register.FullArabicName,
            register.BirthDate,
            register.PhoneNumber,
            register.Address,
            register.CountryId,
            register.CityId,
            register.ProfilePicture,
            ModifiedBy = userId
        };

        await _uow.GlobalActions.ExecuteAsync("usp_User_UpdateProfile", parameters);
    }

    public async Task UpdatePassword(long userId, string newPassword)
    {
        var parameters = new
        {
            UserId = userId,
            NewPassword = newPassword,
            ModifiedBy = userId
        };

        await _uow.GlobalActions.ExecuteAsync("usp_User_UpdatePassword", parameters);
    }

    public async Task SetPasswordResetToken(long userId, string tokenHash, DateTime expiry)
    {
        var parameters = new
        {
            UserId = userId,
            TokenHash = tokenHash,
            Expiry = expiry
        };

        await _uow.GlobalActions.ExecuteAsync("usp_User_SetResetToken", parameters);
    }

    public async Task ResetPassword(long userId, string newPasswordHash)
    {
        var parameters = new
        {
            UserId = userId,
            NewPasswordHash = newPasswordHash
        };

        await _uow.GlobalActions.ExecuteAsync("usp_Auth_ResetPassword", parameters);
    }

    public async Task ClearResetToken(long userId)
    {
        var parameters = new
        {
            UserId = userId
        };

        await _uow.GlobalActions.ExecuteAsync("usp_User_ClearResetToken", parameters);
    }
}