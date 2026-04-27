using Application.DTOs.Requests.Authentication;
using Application.DTOs.Responses.Authentication;
using Application.DTOs.Shared;

namespace Application.Interfaces.Authentication;

public interface IAuthService
{
    // Public Registration (Self-Service) - Returns Token
    Task<ServiceResponse<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    // Login
    Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);

    // Change My Password (Authenticated)
    Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);

    // --- NEW: Recovery Flow ---

    // 1. User requests reset link/code
    Task<ServiceResponse<string>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);

    // 2. User sets new password using token
    Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

}