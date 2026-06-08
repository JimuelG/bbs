namespace API.DTOs;

public class ConcernDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? IncidentLocation { get; set; }
    public string? Purok { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime DateReported { get; set; }
    public DateTime? DateResolved { get; set; }
    public string? ResolutionRemarks { get; set; }

    public int ResidentId { get; set; }
    public string ReporterName { get; set; } = string.Empty;

    public int? AssignedOfficialId { get; set; }
    public string? AssignedOfficialName { get; set; }
}
