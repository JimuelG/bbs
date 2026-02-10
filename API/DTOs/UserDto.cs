using System;

namespace API.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string Contact { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string IdUrl { get; set; } = null!;
    public bool IsIdVerified { get; set; }
    public string? Role { get; set; }
}
