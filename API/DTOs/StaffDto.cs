using System;

namespace API.DTOs;

public class StaffDto
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string? Email { get; set; }
}
