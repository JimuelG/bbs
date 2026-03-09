namespace API.DTOs;

public class UpdateResidentDto
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
}
