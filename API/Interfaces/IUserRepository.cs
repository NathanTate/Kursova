using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserRepository
{
    Task<bool> SaveAllAsync();
    Task<IEnumerable<MemberDto>> GetMembersAsync(string userEmail);
    void DeleteUser(AppUser user);
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByEmailAsync(string email);

}
