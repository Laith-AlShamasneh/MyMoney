namespace Application.DTOs.Requests.Authentication;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
