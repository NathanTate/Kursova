using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ResetPasswordDto
{
    [Required]
    public string ResetToken { get; set; }

    [Required]
    [StringLength(16, MinimumLength = 6)]
    public string Password { get; set; }

    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; }
}
