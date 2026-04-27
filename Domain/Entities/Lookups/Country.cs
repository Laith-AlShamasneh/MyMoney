using Domain.Shared;

namespace Domain.Entities.Lookups;

public class Country : BaseEntity
{
    public int CountryId { get; set; }
    public string IsoCode { get; set; } = null!;
    public string EnglishName { get; set; } = null!;
    public string ArabicName { get; set; } = null!;
}
