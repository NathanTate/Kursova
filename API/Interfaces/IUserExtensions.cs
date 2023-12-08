using System.Security.Claims;

namespace API.Interfaces;

public interface IUserExtensions
{
    string GetEmail(ClaimsPrincipal user);
}

