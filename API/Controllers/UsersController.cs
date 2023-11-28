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
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;
    private readonly IMemoryCache _cache;
    public UsersController(IUserRepository userRepository, IMapper mapper, ILogger<UsersController> logger, IMemoryCache cache)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _cache = cache;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {   IEnumerable<MemberDto> users;
        if(_cache.TryGetValue("users", out users))
        {
            _logger.LogInformation("using cached data");
            return Ok(users);
        }

        _logger.LogInformation("getting new users");
        var currentUser = await _userRepository.GetUserByEmailAsync(User.GetEmail());
        users = await _userRepository.GetMembersAsync(currentUser.Email);

        _cache.Set("users", users, TimeSpan.FromMinutes(1));
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
        _logger.LogInformation("removed users from cache");
        _cache.Remove("users");

        if(await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("Problem deleting user");

    }


}
