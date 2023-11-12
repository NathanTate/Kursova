using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;

    public UsersController(IUserRepository userRepository, IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
    }

    [HttpGet]

    public async Task<IEnumerable<AppUser>> GetUsers()
    {
        return await _userRepository.GetUsersAsync();
    }
}
