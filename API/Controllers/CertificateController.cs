using System.Security.Claims;
using API.DTOs;
using API.RequestHelpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;
using CertificateStatus = Core.Models.CertificateStatus;

namespace API.Controllers;

public class CertificateController(IUnitOfWork unit,
    IMapper mapper, ICertificatePdfService pdfService, UserManager<AppUser> userManager) : BaseApiController
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CreateCertificateDto>> CreateCertificate([FromBody] CreateCertificateDto dto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return Unauthorized("User email claim not found");

        var user = await userManager.Users
            .Include(x => x.Resident)
            .FirstOrDefaultAsync(x => x.Email == email);
        
        if (user == null) return Unauthorized("User not found");

        if (user.Resident == null) return BadRequest("No resident profile is linked to this account.");

        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        var certificate = new BarangayCertificate
        {
            ResidentId = user.Resident.Id,
            FullName = dto.FullName,
            Address = dto.Address,
            CertificateType = dto.CertificateType,
            Purpose = dto.Purpose,
            Fee = dto.Fee,

            Age = dto.Age,
            BirthDate = dto.BirthDate,
            PlaceOfBirth = dto.PlaceOfBirth,
            CivilStatus = dto.CivilStatus,
            Gender = dto.Gender,
            Purok = dto.Purok,
            StayDuration = dto.StayDuration,
            Status = CertificateStatus.Pending,

            IsPaid = false,
            IssuedBy = User.Identity?.Name ?? "System",
            ReferenceNumber = $"BRGY-{phTime:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
        };

        unit.Repository<BarangayCertificate>().Add(certificate);

        if (await unit.Complete())
        {  
            var mapped = mapper.Map<CertificateResponseDto>(certificate);
            return Ok( new{  data = mapped });
        }

        return BadRequest("Problem creating the certificate.");
    }


    [HttpGet("{referenceNumber}/generate-pdf")]
    public async Task<ActionResult> GeneratePdf(string referenceNumber)
    {   
        var spec = new CertificateByReferenceSpecification(referenceNumber);
        var certificate = await unit.Repository<BarangayCertificate>().GetEntityWithSpec(spec);

        if (certificate == null) return NotFound("Certificate not found.");

        var pdfUrl = await pdfService.GenerateCertificatePdfAsync(certificate);

        return Ok(new { PdfUrl = pdfUrl });
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{referenceNumber}/print")]
    public async Task<ActionResult> PrintCertificate(string referenceNumber)
    {
        var spec = new CertificateByReferenceSpecification(referenceNumber);
        var certificate = await unit.Repository<BarangayCertificate>().GetEntityWithSpec(spec);

        if (certificate == null)
            return NotFound("Certificate not found.");

        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        certificate.IssuedAt = phTime;
        certificate.Status = CertificateStatus.Printed;

        var pdfUrl = await pdfService.GenerateCertificatePdfAsync(certificate);

        if (await unit.Complete())
        {
            return Ok(new
            {
                pdfUrl,
                issuedAt = certificate.IssuedAt,
                status = certificate.Status
            });
        }

        return BadRequest("Failed to print certificate");
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("{referenceNumber}/preview-pdf")]
    public async Task<ActionResult> PreviewPdf(string referenceNumber)
    {
        var spec = new CertificateByReferenceSpecification(referenceNumber);
        var certificate = await unit.Repository<BarangayCertificate>().GetEntityWithSpec(spec);

        if (certificate == null)
            return NotFound("Certificate not found.");

        var prediewDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        certificate.IssuedAt = prediewDate;

        var pdfBytes = await pdfService.GenerateCertificatePdfBytesAsync(certificate);

        return File(
            pdfBytes,
            "application/pdf",
            enableRangeProcessing: true
        );
    }


    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCertificate(int id, [FromBody] CreateCertificateDto dto)
    {
        var certificate = await unit.Repository<BarangayCertificate>().GetByIdAsync(id);

        if (certificate == null) return NotFound("Certificate not found.");

        certificate.FullName = dto.FullName;
        certificate.Address = dto.Address;
        certificate.CertificateType = dto.CertificateType;
        certificate.Purpose = dto.Purpose;
        certificate.Fee = dto.Fee;
        certificate.PlaceOfBirth = dto.PlaceOfBirth;
        certificate.Gender = dto.Gender;
        certificate.BirthDate = dto.BirthDate;
        certificate.Purok = dto.Purok;
        certificate.StayDuration = dto.StayDuration;

        var pdfUrl = await pdfService.GenerateCertificatePdfAsync(certificate);

        unit.Repository<BarangayCertificate>().Update(certificate);

        if (await unit.Complete())
        {
            var mapped = mapper.Map<CertificateResponseDto>(certificate);
            return Ok(new
            {
                data = mapped,
                PdfUrl = pdfUrl
            });
        }

        return BadRequest("Problem updating the certificate.");
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult> UpdateCertificateStatus(
        int id,
        UpdateCertificateStatusDto dto)
    {
        var certificate = await unit.Repository<BarangayCertificate>().GetByIdAsync(id);

        if (certificate == null)
            return NotFound("Certificate not found.");

        var allowedStatuses = GetAllowedNextStatuses(certificate.Status);

        if (!allowedStatuses.Contains(dto.Status))
            return BadRequest($"Cannot update certificate from '{certificate.Status}' to '{dto.Status}'.");

        if (dto.Status == CertificateStatus.Printed)
        {
            var phTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time")
            );

            certificate.IssuedAt = phTime;
        }

        certificate.Status = dto.Status;

        unit.Repository<BarangayCertificate>().Update(certificate);

        if (await unit.Complete())
        {
            return Ok(new
            {
               message = "Certificate status updated successfully",
               status = certificate.Status,
               issuedAt = certificate.IssuedAt 
            });
        }

        return BadRequest("Failed to update certificate status.");
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CertificateResponseDto>>> GetCertificate([FromQuery] CertificateSpecParams specParams)
    {
        var spec = new CertificateSpecification(specParams);
        var countSpec = new CertificateCOuntSpecification(specParams);

        var totalItems = await unit.Repository<BarangayCertificate>().CountAsync(countSpec);
        var certificate = await unit.Repository<BarangayCertificate>().ListAsync(spec);

        var data = mapper.Map<IReadOnlyList<BarangayCertificate>, IReadOnlyList<CertificateResponseDto>>(certificate);

        return Ok(new Pagination<CertificateResponseDto>(
            specParams.PageIndex,
            specParams.PageSize,
            totalItems,
            data
        ));
    }

    [HttpPost("upload-signature")]
    public async Task<IActionResult> UploadSignature(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Invalid file.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "signatures");

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

    private static string[] GetAllowedNextStatuses(string? currentStatus)
    {
        return currentStatus switch
        {
            CertificateStatus.Pending => new[]
            {
                CertificateStatus.Pending,
                CertificateStatus.Approved,
                CertificateStatus.Denied
            },
            CertificateStatus.Approved => new[]
            {
                CertificateStatus.Approved,
                CertificateStatus.Printed,
                CertificateStatus.Denied
            },
            CertificateStatus.Printed => new[]
            {
                CertificateStatus.Printed,
                CertificateStatus.Released
            },
            CertificateStatus.Denied => new[]
            {
                CertificateStatus.Denied
            },

            _ => new[]
            {
                CertificateStatus.Pending,
                CertificateStatus.Approved,
                CertificateStatus.Denied
            }
        };
    }
}