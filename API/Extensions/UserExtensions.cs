using API.Interfaces;
using System.Security.Claims;

namespace API.Extensions;

public class UserExtensions : IUserExtensions
{
    public string GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Name)?.Value;
    }
}

