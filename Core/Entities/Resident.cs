namespace Core.Entities;

public class Resident : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string? PictureUrl { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public bool IsHeadOfFamily { get; set; }
    public decimal MonthlyIncome { get; set; }
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}
