namespace Core.Entities;

public class BarangayOfficial : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int Rank { get; set; }
    public bool IsActive { get; set; } = true;
    public string? OfficeImage { get; set; }
    public string? SignatureImage { get; set; }
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; }
}
