namespace API.DTOs;

public class CreateOfficialDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int Rank { get; set; }
}
