namespace Application.DTOs.Requests.Authentication;

public record LoginRequest(string UsernameOrEmail, string Password);