using System;

namespace API.DTOs;

public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
