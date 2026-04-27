namespace Application.DTOs.Responses.Authentication;

public record LoginResponse
{
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? ProfilePictureUrl { get; set; }
    public string AccessToken { get; set; } = null!;
    public IReadOnlyList<RoleResponse> Roles { get; set; } = [];
}
