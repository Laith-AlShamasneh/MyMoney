using Domain.Shared;

namespace Domain.Entities.Authentication;

public class RegisterVM
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullEnglishName { get; set; } = null!;
    public string FullArabicName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int CountryId { get; set; }
    public int CityId { get; set; }
    public string? ProfilePicture { get; set; }
    public UserType UserType { get; set; } = UserType.Normal;
    public int LanguageId { get; set; }
}

public class LoginVM
{
    public long UserId { get; set; }
    public bool IsSuccess { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int LanguageId { get; set; }
}

public class RegisterLoginResultVM
{
    public long UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public long PersonId { get; set; }
    public string FullEnglishName { get; set; } = null!;
    public string FullArabicName { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    public string RoleIds { get; set; } = null!;
    public string RoleNames { get; set; } = null!;
}