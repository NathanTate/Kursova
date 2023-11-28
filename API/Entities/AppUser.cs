namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public string VerificationToken { get; set; }
}
