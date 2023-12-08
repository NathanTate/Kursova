using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace API.Controllers;

[Authorize(Roles = $"{nameof(UserRoles.admin)}, {nameof(UserRoles.teacher)}")]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IUserExtensions _userExtensions;
    private readonly IMemoryCache _cache;
    public UsersController(IUserRepository userRepository, IUserExtensions userExtensions, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _userExtensions = userExtensions;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {   IEnumerable<MemberDto> users;
        if(_cache.TryGetValue("users", out users))
        {
            return Ok(users);
        }
        var currentUser = await _userRepository.GetUserByEmailAsync(_userExtensions.GetEmail(User));
        users = await _userRepository.GetMembersAsync(currentUser.Email);

        _cache.Set("users", users, TimeSpan.FromSeconds(10));
        return Ok(users);
    }

    [HttpPut]
    [Authorize(Roles = nameof(UserRoles.admin))]
    public async Task<ActionResult> UpdateRole(MemberUpdateDto memberUpdateDto)
    {
        string role;

        if(Enum.IsDefined(typeof(UserRoles), memberUpdateDto.RoleId))
        {
            role = ((UserRoles)memberUpdateDto.RoleId).ToString();
        }
        else
        {
            return BadRequest("Such role doesn't exist");
        }
        var user = await _userRepository.GetUserByEmailAsync(memberUpdateDto.Email);
    
        if(user == null) return NotFound();
        
        user.Role = role;
        _cache.Remove("users");
        
        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }

    [HttpDelete("delete/{email}")]
    [Authorize(Roles = nameof(UserRoles.admin))]
    public async Task<ActionResult> DeleteUser(string email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if(user == null) return NotFound();

        _userRepository.DeleteUser(user);
        _cache.Remove("users");

        if(await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem deleting user");

    }


}
