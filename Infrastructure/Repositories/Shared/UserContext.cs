using Domain.Interfaces.Shared;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Repositories.Shared;

public class UserContext(IHttpContextAccessor accessor) : IUserContext
{
    private HttpContext? HttpContext => accessor.HttpContext;

    public bool IsAuthenticated => HttpContext?.User.Identity?.IsAuthenticated == true;

    public long UserId
    {
        get
        {
            if (!IsAuthenticated) return 0;

            var idClaim = HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier);

            return long.TryParse(idClaim?.Value, out var id) ? id : 0;
        }
    }

    public long PersonId
    {
        get
        {
            if (!IsAuthenticated) return 0;

            var personIdClaim = HttpContext!.User.FindFirst("PersonId");

            return long.TryParse(personIdClaim?.Value, out var id) ? id : 0;
        }
    }

    public int LangId
    {
        get
        {
            if (HttpContext is not null &&
                HttpContext.Request.Headers.TryGetValue("Accept-Language", out var langHeader))
            {
                var rawLang = langHeader.ToString().Split(',')[0].Trim(); // Handle "en-US,en;q=0.9"
                if (TryParseLang(rawLang, out var detectedLang))
                {
                    return (int)detectedLang;
                }
            }

            return (int)Languages.Ar;
        }
    }

    public Languages Language => (Languages)LangId;

    public string IpAddress => HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

    public string UserAgent => HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";

    private static bool TryParseLang(string raw, out Languages lang)
    {
        // Handle "en-US" -> "en"
        if (raw.Contains('-'))
        {
            raw = raw.Split('-')[0];
        }

        // Case-insensitive parsing
        if (Enum.TryParse<Languages>(raw, true, out var result) && Enum.IsDefined(result))
        {
            lang = result;
            return true;
        }

        lang = default;
        return false;
    }
}