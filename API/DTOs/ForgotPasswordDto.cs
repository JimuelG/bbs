using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ForgotPasswordDto
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Otp { get; set; } = string.Empty;
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}
