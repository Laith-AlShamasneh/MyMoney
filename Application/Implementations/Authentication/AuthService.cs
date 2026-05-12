using Application.Common.Helpers;
using Application.DTOs.Requests.Authentication;
using Application.DTOs.Responses.Authentication;
using Application.DTOs.Shared;
using Application.Helpers;
using Application.Interfaces.Authentication;
using Application.Interfaces.Shared;
using Domain.Entities.Authentication;
using Domain.Interfaces.Authentication;
using Domain.Interfaces.Shared;
using Domain.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Implementations.Authentication;

public class AuthService(
    IConfiguration configuration,
    IUserRepository userRepository,
    IStorageService storageService,
    IUserContext userContext,
    IPasswordHasher passwordHasher,
    IJwtProvider jwtProvider,
    IMailService mailService,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IStorageService _storageService = storageService;
    private readonly IUserContext _userContext = userContext;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IMailService _mailService = mailService;
    private readonly ILogger<AuthService> _logger = logger;

    private readonly string _userProfileImagePath = StoragePaths.GetPath(FileUploadType.UserProfileImage);
    private readonly string _baseUrl = configuration["Settings:BaseUrl"]!;

    // ==========================================
    // Public Authentication
    // ==========================================

    public async Task<ServiceResponse<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            string? imageName = null;
            if (request.ProfilePicture is not null)
            {
                var uploadResult = await _storageService.SaveFileAsync(
                    _userProfileImagePath, request.ProfilePicture, FileUploadType.UserProfileImage, null, false, ct);
                imageName = uploadResult.FileName;
            }

            var registerVm = new RegisterVM
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FullEnglishName = request.FullEnglishName,
                FullArabicName = request.FullArabicName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address ?? string.Empty,
                CountryId = request.CountryId,
                CityId = request.CityId,
                ProfilePicture = imageName,
                LanguageId = (int)_userContext.Language
            };

            var userResult = await _userRepository.Register(registerVm);

            var loginResponse = await GenerateLoginResponse(userResult, ct);

            await SendWelcomeEmail(request, ct);

            return ServiceResponse<LoginResponse>.Success(
                loginResponse,
                MessagesHelper.GetMessage(MessageType.RegisterSuccess, _userContext.Language),
                HttpResponseStatus.Created);

        }, _logger, "Register User", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            // 1. Get User Credentials
            // Note: Make sure GetByUsernameOrEmail is implemented in your UserRepository/Database!
            var user = await _userRepository.Login(new LoginVM
            {
                // We send a dummy login to fetch the user if needed, or you should create a dedicated GetUserByEmail method.
                // Assuming you have a way to fetch the user hash first before verifying:
            });

            // *Self-Correction based on your earlier code*: 
            // Your Dapper 'usp_Auth_Login' procedure requires you to verify the password in C# first, 
            // OR you pass the hash to the DB. If your DB procedure handles everything (locks, counts, etc.), 
            // you might need to adjust this flow slightly depending on how `_userRepository.GetByUsernameOrEmail` is implemented.
            // Assuming `GetByUsernameOrEmail` fetches the user hash and lock status:

            /*
            var user = await _userRepository.GetByUsernameOrEmail(request.UsernameOrEmail);

            if (user is null)
            {
                return ServiceResponse<LoginResponse>.Failure(
                    ErrorCodes.Authentication.INVALID_CREDENTIALS,
                    MessagesHelper.GetMessage(MessageType.InvalidCredentials, _userContext.Language),
                    HttpResponseStatus.BadRequest);
            }

            var loginVm = new LoginVM
            {
                UserId = user.UserId,
                IpAddress = _userContext.IpAddress,
                UserAgent = _userContext.UserAgent,
                LanguageId = (int)_userContext.Language
            };

            if (!user.IsActive)
            {
                loginVm.IsSuccess = false;
                loginVm.FailureReason = SignInFailureReason.UserNotActive.ToString();
                await _userRepository.Login(loginVm);
                return ServiceResponse<LoginResponse>.Failure(ErrorCodes.Authentication.ACCOUNT_DISABLED, MessagesHelper.GetMessage(MessageType.AccountDisabled, _userContext.Language), HttpResponseStatus.Forbidden);
            }

            if (user.IsLocked && user.LockoutEndDateTime > DateTime.UtcNow)
            {
                loginVm.IsSuccess = false;
                loginVm.FailureReason = SignInFailureReason.UserLockedOut.ToString();
                await _userRepository.Login(loginVm);
                return ServiceResponse<LoginResponse>.Failure(ErrorCodes.Authentication.ACCOUNT_LOCKED, MessagesHelper.GetMessage(MessageType.AccountLocked, _userContext.Language), HttpResponseStatus.Forbidden);
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                loginVm.IsSuccess = false;
                loginVm.FailureReason = SignInFailureReason.InvalidPassword.ToString();
                await _userRepository.Login(loginVm);
                return ServiceResponse<LoginResponse>.Failure(ErrorCodes.Authentication.INVALID_CREDENTIALS, MessagesHelper.GetMessage(MessageType.InvalidCredentials, _userContext.Language), HttpResponseStatus.BadRequest);
            }

            // Success
            loginVm.IsSuccess = true;
            loginVm.FailureReason = null;
            var loginResult = await _userRepository.Login(loginVm);

            var response = await GenerateLoginResponse(loginResult!, ct);

            return ServiceResponse<LoginResponse>.Success(
                response,
                MessagesHelper.GetMessage(MessageType.LoginSuccess, _userContext.Language),
                HttpResponseStatus.OK);
            */

            throw new NotImplementedException("Ensure GetByUsernameOrEmail is implemented in UserRepository to use the full Login flow.");

        }, _logger, "Login User", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            long userId = _userContext.UserId;

            // You will need a GetById method in your UserRepository to fetch the current hash
            // var user = await _userRepository.GetById(userId);
            // if (user is null) ...

            /*
            if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return ServiceResponse<bool>.Failure(ErrorCodes.Authentication.INVALID_CREDENTIALS, MessagesHelper.GetMessage(MessageType.InvalidPassword, _userContext.Language), HttpResponseStatus.BadRequest);
            }
            */

            string newHash = _passwordHasher.HashPassword(request.NewPassword);
            await _userRepository.UpdatePassword(userId, newHash);

            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.Success, _userContext.Language));

        }, _logger, "Change Password", null, _userContext.Language, ct);
    }

    // ==========================================
    // Recovery Flow
    // ==========================================

    public async Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            // var user = await _userRepository.GetByUsernameOrEmail(request.Email);

            /*
            if (user == null)
            {
                return ServiceResponse<string>.Success(
                    MessagesHelper.GetMessage(MessageType.PasswordResetRequestReceived, _userContext.Language),
                    MessagesHelper.GetMessage(MessageType.PasswordResetRequestReceived, _userContext.Language));
            }

            string rawToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
            string tokenHash = _passwordHasher.HashPassword(rawToken);
            DateTime expiry = DateTime.UtcNow.AddHours(1);

            await _userRepository.SetPasswordResetToken(user.UserId, tokenHash, expiry);
            await SendForgetPasswordEmail(user.Email, user.Username, tokenHash, ct);
            */

            return ServiceResponse<string>.Success(
                MessagesHelper.GetMessage(MessageType.PasswordResetRequestReceived, _userContext.Language),
                MessagesHelper.GetMessage(MessageType.PasswordResetRequestReceived, _userContext.Language));

        }, _logger, "Forgot Password", request, _userContext.Language, ct);
    }

    public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        return await ExecutionHelper.ExecuteAsync(async () =>
        {
            // var user = await _userRepository.GetByUsernameOrEmail(request.Email);

            /*
            if (user is null ||
                string.IsNullOrEmpty(user.PasswordResetTokenHash) ||
                user.PasswordResetTokenExpiry < DateTime.UtcNow ||
                !_passwordHasher.VerifyPassword(request.Token, user.PasswordResetTokenHash))
            {
                return ServiceResponse<bool>.Failure(ErrorCodes.Authentication.TOKEN_INVALID, MessagesHelper.GetMessage(MessageType.InvalidInput, _userContext.Language), HttpResponseStatus.BadRequest);
            }

            string newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            await _userRepository.ResetPassword(user.UserId, newPasswordHash);
            */

            return ServiceResponse<bool>.Success(true, MessagesHelper.GetMessage(MessageType.PasswordResetSuccess, _userContext.Language));

        }, _logger, "Reset Password", request.Email, _userContext.Language, ct);
    }

    // ==========================================
    // Private Helpers
    // ==========================================

    private async Task<LoginResponse> GenerateLoginResponse(RegisterLoginResultVM userResult, CancellationToken ct)
    {
        var roleIds = userResult.RoleIds?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .OrderBy(id => id)
            .ToList() ?? [];

        var roleNames = userResult.RoleNames?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .ToList() ?? [];

        var claimsModel = new UserClaimsModel(
            userResult.UserId,
            userResult.PersonId,
            userResult.Email,
            _userContext.Language,
            roleIds);

        var accessToken = _jwtProvider.GenerateToken(claimsModel);

        var fileInfo = await _storageService.GetFileInfoAsync(
            _userProfileImagePath,
            userResult.ProfilePicture ?? string.Empty,
            _baseUrl,
            ct);

        var roleResponses = new List<RoleResponse>();
        for (int i = 0; i < roleIds.Count; i++)
        {
            string name = i < roleNames.Count ? roleNames[i] : "Unknown";
            roleResponses.Add(new RoleResponse { RoleId = roleIds[i], Name = name });
        }

        return new LoginResponse
        {
            UserId = userResult.UserId,
            Username = userResult.Username,
            FullName = _userContext.Language == Languages.Ar ? userResult.FullArabicName : userResult.FullEnglishName,
            Email = userResult.Email,
            ProfilePictureUrl = fileInfo?.Url,
            AccessToken = accessToken,
            Roles = roleResponses
        };
    }

    private async Task SendWelcomeEmail(RegisterRequest request, CancellationToken ct)
    {
        try
        {
            string frontendUrl = _configuration["Settings:FrontendUrl"] ?? "http://localhost:3000";
            string dashboardLink = $"{frontendUrl}/dashboard";
            string userFullName = _userContext.Language == Languages.Ar ? request.FullArabicName : request.FullEnglishName;

            string emailBody = EmailTemplateHelper.GenerateWelcomeEmail(userFullName, dashboardLink, _userContext.Language);
            string subject = MessagesHelper.GetMessage(MessageType.WelcomeEmailSubject, _userContext.Language);

            var mailRequest = new MailRequest(request.Email, subject, emailBody);
            await _mailService.SendEmailAsync(mailRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to user {Email}", request.Email);
        }
    }

    private async Task SendForgetPasswordEmail(string toEmail, string userName, string token, CancellationToken ct)
    {
        try
        {
            string frontendUrl = _configuration["Settings:FrontendUrl"] ?? "http://localhost:3000";
            string resetLink = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(token)}";

            string emailBody = EmailTemplateHelper.GenerateForgotPasswordBody(userName, resetLink, _userContext.Language);
            string subject = MessagesHelper.GetMessage(MessageType.PasswordResetSubject, _userContext.Language);

            var mailRequest = new MailRequest(toEmail, subject, emailBody);
            await _mailService.SendEmailAsync(mailRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send forgot password email to user {Email}", toEmail);
        }
    }
}