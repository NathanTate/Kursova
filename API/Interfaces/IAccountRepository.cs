using API.DTOs;
using API.Entities;

namespace API;

public interface IAccountRepository
{
    Task<bool> UserExists(string email);
    Task SaveAllAsync();
    Task<AppUser> GetUserByEmailAsync(string email);
    Task<AppUser> GetUserByTokenAsync(string token);
    AppUser Register(RegisterDto registerDto);
    string CreateRandomToken();
    Task SendVerificationEmail(string email, string verificationToken);
    Task SendVerificationPassword(string email, string verificationToken);
}
