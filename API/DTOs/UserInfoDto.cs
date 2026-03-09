namespace API.DTOs;

public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public int ResidentId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? PictureUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public string IdUrl { get; set; } = string.Empty;
    public bool IsIdVerified { get; set; }
}
