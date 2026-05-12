namespace Application.DTOs.Responses.Authentication;

public record RoleResponse
{
    public long RoleId { get; set; }
    public string Name { get; set; } = null!;
}
