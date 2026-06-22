using Core.Enums;
using Core.Models;

namespace Core.Entities;

public class BarangayCertificate : BaseEntity
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public int? Age { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string CivilStatus { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string StayDuration { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public DateTime? IssuedAt { get; set; }
    public string IssuedBy { get; set; } = string.Empty;
    public decimal? Fee { get; set; }
    public bool IsPaid { get; set; }
    public int? ResidentId { get; set; }
    public Resident? Resident { get; set; } = null!;
    public string Status { get; set; } = CertificateStatus.Pending;
}
