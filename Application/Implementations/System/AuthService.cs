using Application.DTOs.Requests.Authentication;
using Application.DTOs.Responses.Authentication;
using Application.DTOs.Shared;
using Application.Interfaces.Authentication;
using Application.Interfaces.Shared;
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

    public Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
