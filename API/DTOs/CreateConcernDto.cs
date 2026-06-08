namespace API.DTOs;

public class CreateConcernDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? IncidentLocation { get; set; }
    public string? Purok { get; set; }
    public int ResidentId { get; set; }
}
