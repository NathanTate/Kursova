using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IEmailSender _emailSender;

    public AccountController(DataContext context, ITokenService tokenService, 
        IMapper mapper, IEmailSender emailSender)
    {
        _context = context;
        _tokenService = tokenService;
        _mapper = mapper;
        _emailSender = emailSender;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await UserExists(registerDto.Email)) return BadRequest("Email is already taken");

        var user = _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();

            user.Email = registerDto.Email.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            user.IsEmailVerified = false;
            user.VerificationToken = CreateRandomToken();

         _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await SendVerificationEmail(user.Email, user.VerificationToken);

        return new UserDto
        {
            Email = user.Email,
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        if(loginDto.Email == null) return Unauthorized("Enter an email");

        var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email.ToLower());

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
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("verify-email")]

    public async Task<IActionResult> VerifyEmail([FromQuery] VerificationParams verificationParams)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == verificationParams.Email.ToLower());
        if(user == null) return BadRequest("User doesn't exist");
        if(user.VerificationToken != verificationParams.Token) return BadRequest("Invalid Token");

        user.IsEmailVerified = true;
        user.VerificationToken = null;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Email is verified" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDto.Email.ToLower());

        if(user == null) return BadRequest("User not found");
        if(user.IsEmailVerified == false) return BadRequest("Verification needed");

        user.VerificationToken = CreateRandomToken();
        await _context.SaveChangesAsync();

        await SendVerificationPassword(user.Email, user.VerificationToken);

        return Ok(new {message = "You may reset your password now"});
    }

    [HttpPut("reset-password")]

    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == resetPasswordDto.Token);

        if(user == null) return BadRequest("User not found");

        using var hmac = new HMACSHA512();
        
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(resetPasswordDto.Password));
        user.PasswordSalt = hmac.Key;
        user.VerificationToken = null;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
    }

    private string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public async Task<ActionResult> SendVerificationEmail(string email, string verificationToken)
    {
        var receiver = email;
        var subject = "Email Verification";
        var message = $"Please click <a href=\"https://localhost:4200/verify-email?email={email}&token={verificationToken}\"> to verify email</a>";

        await _emailSender.SendEmailAsync(receiver, subject, message);

        return Ok();
    }

    public async Task<ActionResult> SendVerificationPassword(string email, string verificationToken)
    {
        var receiver = email;
        var subject = "Password Verification";
        var message = $"Enter the code to reset password {verificationToken}";

        await _emailSender.SendEmailAsync(receiver, subject, message);

        return Ok();
    }
}
