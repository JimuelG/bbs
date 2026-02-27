using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CertificateController(IUnitOfWork unit,
    IMapper mapper, ICertificatePdfService pdfService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<CreateCertificateDto>> CreateCertificate([FromBody] CreateCertificateDto dto)
    {
        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        var certificate = new BarangayCertificate
        {
            FullName = dto.FullName,
            Address = dto.Address,
            CertificateType = dto.CertificateType,
            Purpose = dto.Purpose,
            Fee = dto.Fee,
        
            BirthDate = dto.BirthDate,
            CivilStatus = dto.CivilStatus,
            Purok = dto.Purok,
            StayDuration = dto.StayDuration,

            IsPaid = false,
            IssuedAt = phTime,
            IssuedBy = User.Identity?.Name ?? "System",
            ReferenceNumber = $"BRGY-{phTime:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
        };

        var pdfUrl = await pdfService.GenerateCertificatePdfAsync(certificate);

        unit.Repository<BarangayCertificate>().Add(certificate);
        await unit.Complete();

        var mapped = mapper.Map<CertificateResponseDto>(certificate);

        return Ok( new
        {   
            data = mapped,
            PdfUrl = pdfUrl
        });
    }

    // [Authorize(Roles = "Staff")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CertificateResponseDto>>> GetCertificate()
    {
        var certificate = await unit.Repository<BarangayCertificate>().ListAllAsync();

        if (certificate == null) return NotFound("No Certificate found");

        return Ok(mapper.Map<IReadOnlyList<CertificateResponseDto>>(certificate));
    }

    [HttpPost("upload-signature")]
    public async Task<IActionResult> UploadSignature(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "signatures");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, "current.png");

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { Message = "Signature updaloed successfully. "});
    }

    [HttpGet("verify/{referenceNumber}")]
    public async Task<IActionResult> Verify(string referenceNumber)
    {
        var cert = await unit.Repository<BarangayCertificate>()
            .GetEntityWithSpec(new CertificateByReferenceSpecification(referenceNumber));

        if (cert == null) return NotFound(new { Valid = false, Message = "Certificate not found."});

        return Ok (new
        {
            Valid = true,
            cert.FullName,
            cert.CertificateType,
            cert.ReferenceNumber 
        });
    }

    [HttpGet("/certificates/verify/{referenceNumber}")]
    public async Task<IActionResult> PublicVerify(string referenceNumber)
    {
        var cert = await unit.Repository<BarangayCertificate>()
                        .GetEntityWithSpec(new CertificateByReferenceSpecification(referenceNumber));

        if (cert == null)
        {
            return Content(@"
                <html>
                    <body style='font-family:Arial;text-align:center:padding:40px'>
                        <h1 style='color:red'>INVALID CERTIFICATE</h1>
                        <p>This certificate does not exist in our records.</p>
                    </body>
                </html>", "text/html");
        }

        return Content($@"
            <html>
                <body style='font-family:Arial;text-align:center;padding:40px'>
                <h1 style='color:green'>VALID CERTIFICATE</h1>
                <hr style='width:300px'/>
                <p><strong>Name:</strong> {cert.FullName}</p>
                <p><strong>Type:</strong> {cert.CertificateType}</p>
                <p><strong>Issued:</strong> {cert.IssuedAt:MMMM dd, yyyy}</p>
                <p><strong>Reference:</strong> {cert.ReferenceNumber}</p>
                </body>
            </html>", "text/html");
    }
}