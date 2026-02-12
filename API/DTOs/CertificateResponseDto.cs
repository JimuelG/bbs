using Core.Enums;

namespace API.DTOs;

public class CertificateResponseDto
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public CertificateType CertificateType { get; set; }
    public DateTime IssuedAt { get; set; }
    public decimal Fee { get; set; }
    public bool IsPaid { get; set; }
}
