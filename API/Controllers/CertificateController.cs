using System.Runtime.ConstrainedExecution;
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CertificateController(IUnitOfWork unit,
    IMapper mapper, ICertificatePdfService pdfService) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<CreateCertificateDto>> CreateCertificate(CreateCertificateDto dto)
    {
        var certificate = new BarangayCertificate
        {
            FullName = dto.FullName,
            Address = dto.Address,
            CertificateType = dto.CertificateType,
            Purpose = dto.Purpose,
            Fee = dto.Fee,
            IsPaid = true,
            IssuedAt = DateTime.UtcNow,
            IssuedBy = User.Identity?.Name ?? "System",
            ReferenceNumber = $"BRGY-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
        };

        var pdfUrl = await pdfService.GenerateCertificatePdfAsync(certificate);

        unit.Repository<BarangayCertificate>().Add(certificate);
        await unit.Complete();

        var mapped = mapper.Map<CertificateResponseDto>(certificate);

        return Ok( new
        {   
            mapped,
            PdfUrl = pdfUrl
        });

    }

    [Authorize(Roles = "Staff")]
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
}