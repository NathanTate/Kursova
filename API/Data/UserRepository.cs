using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MemberDto>> GetMembersAsync(string userEmail)
    {
        var query = _context.Users.Where(u => u.Email != userEmail);
        
        return await query
       .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
       .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void DeleteUser(AppUser user)
    {
        _context.Users.Remove(user);
    }

    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        return await _context.Users
        .SingleOrDefaultAsync(x => x.Email == email.ToLower());
    }
}
