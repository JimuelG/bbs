namespace API.DTOs;

public class UpdateConcernDto
{
    public string Status { get; set; } = string.Empty;
    public int? AssignedOfficalId { get; set; }
    public string? ResolutionRemarks { get; set; }
}
