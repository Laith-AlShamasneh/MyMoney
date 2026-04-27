namespace Application.DTOs.Shared;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";
    public string SecretKey { get; set; } = string.Empty;
    public string IssuerKey { get; set; } = string.Empty;
    public string AudienceKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}