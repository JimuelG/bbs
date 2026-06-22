using Core.Enums;

namespace API.DTOs;

public class CertificateResponseDto
{
    public int Id { get; set; }
    public string ResidentId { get; set; }  = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public decimal Fee { get; set; }
    public int? Age { get; set; }
    public DateTime? BirthDate { get; set; }
    public string CivilStatus { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string StayDuration { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime IssuedAt { get; set; }
    public bool IsPaid { get; set; }
}
