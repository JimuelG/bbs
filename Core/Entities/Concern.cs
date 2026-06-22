using System;
using Core.Enums;

namespace Core.Entities;

public class Concern : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConcernType Type { get; set; }
    public ConcernStatus Status { get; set; } = ConcernStatus.Pending;
    public string? IncidentLocation { get; set; }
    public string? Purok { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime DateReported { get; set; } = DateTime.UtcNow;
    public DateTime? DateResolved { get; set; }
    public string? ResolutionRemarks { get; set; }
    public ConcernPriority Priority { get; set; } = ConcernPriority.P3;
    public int ResidentId { get; set; }
    public Resident Resident { get; set; } = null!;

    public int? AssignedOfficialId { get; set; }
    public BarangayOfficial? AssignedOfficial { get; set; }
}
