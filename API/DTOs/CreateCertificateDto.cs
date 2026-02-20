using Core.Enums;

namespace API.DTOs;

public class CreateCertificateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public decimal? Fee { get; set; }
    public DateTime BirthDate { get; set; }
    public string CivilStatus { get; set; } = string.Empty;
    public string Purok { get; set; } = string.Empty;
    public string StayDuration { get; set; } = string.Empty;

}
