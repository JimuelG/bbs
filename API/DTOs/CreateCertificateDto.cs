using Core.Enums;
using Core.Models;

namespace API.DTOs;

public class CreateCertificateDto
{
    public int ResidentId { get; set; } 
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? PlaceOfBirth { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
    public decimal? Fee { get; set; }
    public DateTime? BirthDate { get; set; }
    public string CivilStatus { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string StayDuration { get; set; } = string.Empty;

}
