using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITokenService _tokenService;
    public AccountController(IAccountRepository accountRepository, ITokenService tokenService)
    {
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await _accountRepository.UserExists(registerDto.Email)) return BadRequest("Email is already taken");

        var user = _accountRepository.Register(registerDto);

        await _accountRepository.SaveAllAsync();

        await _accountRepository.SendVerificationEmail(user.Email, user.VerificationToken);

        return new UserDto
        {
            Email = user.Email,
            Username = user.UserName,
            Role = user.Role,
            Token = user.VerificationToken
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        if(loginDto.Email == null) return Unauthorized("Enter an email");
        var user = await _accountRepository.GetUserByEmailAsync(loginDto.Email);

        if(user == null) return Unauthorized("Invalid email");
        if(user.IsEmailVerified == false) return BadRequest("Confirm your email first");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        for(int i =0; i < computedHash.Length; i++)
        {
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Email = user.Email,
            Username = user.UserName,
            Role = user.Role,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("verify-email")]

    public async Task<IActionResult> VerifyEmail([FromQuery] VerificationParams verificationParams)
    {
        var user = await _accountRepository.GetUserByEmailAsync(verificationParams.Email);
        if(user == null) return BadRequest("User doesn't exist");
        if(user.VerificationToken != verificationParams.Token) return BadRequest("Invalid Token");

        user.IsEmailVerified = true;
        user.VerificationToken = null;

        await _accountRepository.SaveAllAsync();

        return Ok(new { message = "Email is verified" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _accountRepository.GetUserByEmailAsync(forgotPasswordDto.Email);

        if(user == null) return BadRequest("User not found");
        if(user.IsEmailVerified == false) return BadRequest("Verification needed");

        user.VerificationToken = _accountRepository.CreateRandomToken();
        await _accountRepository.SaveAllAsync();

        await _accountRepository.SendVerificationPassword(user.Email, user.VerificationToken);

        return Ok(new {message = "You may reset your password now"});
    }

    [HttpPut("reset-password")]

    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _accountRepository.GetUserByTokenAsync(resetPasswordDto.ResetToken);

        if(user == null) return BadRequest("User not found");

        using var hmac = new HMACSHA512();
        
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDto.Password));
        user.PasswordSalt = hmac.Key;
        user.VerificationToken = null;

        await _accountRepository.SaveAllAsync();
        return NoContent();
    }
}
