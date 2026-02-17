namespace Core.Entities;

public class Resident : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public bool IsHeadOfFamily { get; set; }
    public decimal MonthlyIncome { get; set; }
    public string? AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}
