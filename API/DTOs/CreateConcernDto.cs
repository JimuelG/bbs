using Core.Enums;

namespace API.DTOs;

public class CreateConcernDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConcernType Type { get; set; }
    public string? IncidentLocation { get; set; }
    public string? Purok { get; set; }
}
