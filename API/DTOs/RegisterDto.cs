using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;  
    public string? MiddleName { get; set; }
    [Required]
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required]
    public string Purok { get; set; } = string.Empty;
    public string? IdUrl { get; set; }
}
