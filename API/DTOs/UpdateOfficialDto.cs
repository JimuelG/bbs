namespace API.DTOs;

public class UpdateOfficialDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int Rank { get; set; }
    public bool IsActive { get; set; }
}
