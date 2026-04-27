using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Requests.Authentication;

public record RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    // Person Details
    public string FullEnglishName { get; set; } = null!;
    public string FullArabicName { get; set; } = null!;
    public DateOnly BirthDate { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int CountryId { get; set; }
    public int CityId { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}
