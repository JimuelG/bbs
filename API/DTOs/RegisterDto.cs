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
    public string Contact { get; set; } = string.Empty;
    [Required]
    public string Address { get; set; } = string.Empty;
    [Required]
    public string IdUrl { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
