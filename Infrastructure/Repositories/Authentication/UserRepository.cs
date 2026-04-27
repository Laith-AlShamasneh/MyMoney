using Dapper;
using Domain.Entities.Authentication;
using Domain.Helpers;
using Domain.Interfaces.Authentication;
using Domain.Interfaces.Shared;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Authentication;

internal class UserRepository(IUnitOfWork uow) : IUserRepository
{
    private readonly IUnitOfWork _uow = uow;

    public async Task<RegisterLoginResultVM> Register(RegisterVM register)
    {
        DynamicParameters parameters = new();

        parameters.Add("@Username", register.Username, DbType.String, ParameterDirection.Input);
        parameters.Add("@Email", register.Email, DbType.String, ParameterDirection.Input);
        parameters.Add("@PasswordHash", register.PasswordHash, DbType.String, ParameterDirection.Input);
        parameters.Add("@FullEnglishName", register.FullEnglishName, DbType.String, ParameterDirection.Input);
        parameters.Add("@FullArabicName", register.FullArabicName, DbType.String, ParameterDirection.Input);
        parameters.Add("@BirthDate", register.BirthDate, DbType.Date, ParameterDirection.Input);
        parameters.Add("@PhoneNumber", register.PhoneNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("@Address", register.Address, DbType.String, ParameterDirection.Input);
        parameters.Add("@CountryId", register.CountryId, DbType.Int16, ParameterDirection.Input);
        parameters.Add("@CityId", register.CityId, DbType.Int16, ParameterDirection.Input);
        parameters.Add("@ProfilePicture", register.ProfilePicture, DbType.String, ParameterDirection.Input);
        parameters.Add("@DefaultRoleId", 2, DbType.Int32, ParameterDirection.Input);

        try
        {
            var result = await _uow.GlobalActions.ExecuteSingleAsync<RegisterLoginResultVM>(
                "usp_User_Register",
                parameters);

            return result!;
        }
        catch (SqlException ex) when (ex.Number == 50001)
        {
            throw new SqlExceptionHelper(ex.Number, ex.Message);
        }
    }

    public async Task<RegisterLoginResultVM?> Login(LoginVM login)
    {
        DynamicParameters parameters = new();
        parameters.Add("@UserId", login.UserId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@IsSuccess", login.IsSuccess, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("@FailureReason", login.FailureReason, DbType.String, ParameterDirection.Input);
        parameters.Add("@ClientIpAddress", login.IpAddress, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserAgent", login.UserAgent, DbType.String, ParameterDirection.Input);
        parameters.Add("@LanguageId", login.LanguageId, DbType.Int32, ParameterDirection.Input);

        return await _uow.GlobalActions.ExecuteSingleAsync<RegisterLoginResultVM>(
            "usp_User_Login",
            parameters);
    }

    public async Task UpdateProfile(RegisterVM register, long userId)
    {
        DynamicParameters parameters = new();

        // User Parameters
        parameters.Add("@UserId", userId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@Username", register.Username, DbType.String, ParameterDirection.Input);
        parameters.Add("@Email", register.Email, DbType.String, ParameterDirection.Input);
        parameters.Add("@PasswordHash", register.PasswordHash, DbType.String, ParameterDirection.Input);

        // Person Parameters
        parameters.Add("@FullEnglishName", register.FullEnglishName, DbType.String, ParameterDirection.Input);
        parameters.Add("@FullArabicName", register.FullArabicName, DbType.String, ParameterDirection.Input);
        parameters.Add("@BirthDate", register.BirthDate, DbType.Date, ParameterDirection.Input);
        parameters.Add("@PhoneNumber", register.PhoneNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("@Address", register.Address, DbType.String, ParameterDirection.Input);
        parameters.Add("@CountryId", register.CountryId, DbType.Int16, ParameterDirection.Input);
        parameters.Add("@CityId", register.CityId, DbType.Int16, ParameterDirection.Input);
        parameters.Add("@ProfilePicture", register.ProfilePicture, DbType.String, ParameterDirection.Input);
        parameters.Add("@ModifiedBy", userId, DbType.String, ParameterDirection.Input);

        await _uow.GlobalActions.ExecuteAsync(
            "usp_User_UpdateProfile",
            parameters);
    }

    public async Task UpdatePassword(long userId, string newPassword)
    {
        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@NewPassword", newPassword, DbType.String, ParameterDirection.Input);
        parameters.Add("@ModifiedBy", userId, DbType.Int64, ParameterDirection.Input);

        await _uow.GlobalActions.ExecuteAsync(
            "usp_User_UpdatePassword",
            parameters);
    }

    public async Task SetPasswordResetToken(long userId, string tokenHash, DateTime expiry)
    {
        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@TokenHash", tokenHash, DbType.String, ParameterDirection.Input);
        parameters.Add("@Expiry", expiry, DbType.DateTime2, ParameterDirection.Input);

        await _uow.GlobalActions.ExecuteAsync("usp_User_SetResetToken", parameters);
    }

    public async Task ResetPassword(long userId, string newPasswordHash)
    {
        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@NewPasswordHash", newPasswordHash, DbType.String, ParameterDirection.Input);

        await _uow.GlobalActions.ExecuteAsync("usp_Auth_ResetPassword", parameters);
    }

    public async Task ClearResetToken(long userId)
    {
        DynamicParameters parameters = new();
        parameters.Add("@UserId", userId, DbType.Int64, ParameterDirection.Input);

        await _uow.GlobalActions.ExecuteAsync("usp_User_ClearResetToken", parameters);
    }

}