namespace Application.DTOs.Requests.Authentication;

public record ResetPasswordRequest(string Email, string Token, string NewPassword);
