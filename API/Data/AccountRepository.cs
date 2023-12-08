using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AccountRepository : IAccountRepository
{
    private readonly DataContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public AccountRepository(DataContext context, IEmailSender emailSender, IMapper mapper)
    {
        _context = context;
        _emailSender = emailSender;
        _mapper = mapper;
    }

    public AppUser Register(RegisterDto registerDto)
    {
        var user = _mapper.Map<AppUser>(registerDto);

        using var hmac = new HMACSHA512();

            user.Email = registerDto.Email.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
            user.Role = UserRoles.student.ToString();
            user.IsEmailVerified = false;
            user.VerificationToken = CreateRandomToken();

        _context.Users.Add(user);

        return user;

    }

    public async Task SaveAllAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserExists(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email.ToLower());
    }

    public string CreateRandomToken()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
    }

    public async Task SendVerificationEmail(string email, string verificationToken)
    {
        var receiver = email;
        var subject = "Email Verification";
        var message = $"Please click <a href=\"https://localhost:4200/verify-email?email={email}&token={verificationToken}\"> to verify email</a>";

        await _emailSender.SendEmailAsync(receiver, subject, message);
    }

    public async Task SendVerificationPassword(string email, string verificationToken)
    {
        var receiver = email;
        var subject = "Password Verification";
        var message = $"Enter the code to reset password {verificationToken}";

        await _emailSender.SendEmailAsync(receiver, subject, message);
    }

    public async Task<AppUser> GetUserByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public async Task<AppUser> GetUserByTokenAsync(string token)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
    }
}

