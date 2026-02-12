using System.Runtime.ConstrainedExecution;
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class CertificateController(IUnitOfWork unit,
    IMapper mapper) : BaseApiController
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

        unit.Repository<BarangayCertificate>().Add(certificate);
        await unit.Complete();

        return Ok(mapper.Map<CertificateResponseDto>(certificate));

    }

    [Authorize(Roles = "Staff")]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CertificateResponseDto>>> GetCertificate()
    {
        var certificate = await unit.Repository<BarangayCertificate>().ListAllAsync();

        if (certificate == null) return NotFound("No Certificate found");

        return Ok(mapper.Map<IReadOnlyList<CertificateResponseDto>>(certificate));
    }
}