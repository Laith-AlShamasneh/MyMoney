using Domain.Shared;

namespace Domain.Entities.Lookups;

public class City : BaseEntity
{
    public int CityId { get; set; }
    public int CountryId { get; set; }
    public string EnglishName { get; set; } = null!;
    public string ArabicName { get; set; } = null!;
}
