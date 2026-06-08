using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ConcernsController(IUnitOfWork unit, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConcernDto>>> GetConcerns()
    {
        var spec = new ConcernWithDetailsSpecification();
        var concerns = await unit.Repository<Concern>().ListAsync(spec);

        return Ok(mapper.Map<IReadOnlyList<ConcernDto>>(concerns));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ConcernDto>> GetConern(int id)
    {
        var spec = new ConcernWithDetailsSpecification(id);
        var concern = await unit.Repository<Concern>().GetEntityWithSpec(spec);

        if (concern == null) return NotFound();

        return Ok(mapper.Map<ConcernDto>(concern));
    }

    [HttpGet("resident/{residentId}")]
    public async Task<ActionResult<IReadOnlyList<ConcernDto>>> GetResidentConcerns(int residentId)
    {
        var spec = new ConcernWithDetailsSpecification(residentId, true);
        var concerns = await unit.Repository<Concern>().ListAsync(spec);

        return Ok(mapper.Map<IReadOnlyList<ConcernDto>>(concerns));
    }

    [HttpPost]
    public async Task<ActionResult<ConcernDto>> CreateConcern(CreateConcernDto dto)
    {
        var concern = mapper.Map<Concern>(dto);

        unit.Repository<Concern>().Add(concern);

        if (await unit.Complete())
            return Ok(new { message = "Concern submitted successfully", id = concern.Id});

        return BadRequest(new { message = "Failed to submit concern" });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateConcern(int id, UpdateConcernDto dto)
    {
        var concern = await unit.Repository<Concern>().GetByIdAsync(id);
        if (concern == null) return NotFound("Concern not found");

        if (Enum.TryParse<ConcernStatus>(dto.Status, out var parsedStatus))
        {
            concern.Status = parsedStatus;
    
            if (parsedStatus == ConcernStatus.Resolved || parsedStatus == ConcernStatus.Dismissed)
            {
                concern.DateResolved = DateTime.UtcNow;
            }
        }

        if (dto.AssignedOfficalId.HasValue)
        {
            concern.AssignedOfficialId = dto.AssignedOfficalId.Value;
        }

        if (!string.IsNullOrEmpty(dto.ResolutionRemarks))
        {
            concern.ResolutionRemarks = dto.ResolutionRemarks;
        }
        
        unit.Repository<Concern>().Update(concern);

        if (await unit.Complete())
            return Ok(new { message = "Concern updated successfully" });

        return BadRequest(new { message = "Failed to update concern" });
    }

    [HttpPost("{id}/upload-photo")]
    public async Task<ActionResult> UploadPhoto(int id, IFormFile file)
    {
        var concern = await unit.Repository<Concern>().GetByIdAsync(id);
        if (concern == null) return NotFound("Concern not found");

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Invalid file type. Only JPG and PNG are allowed." });

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "concerns");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        var fileName = $"concern_{id}_{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        concern.PhotoUrl = $"/images/concerns/{fileName}";

        unit.Repository<Concern>().Update(concern);
        await unit.Complete();

        return Ok(new { url = concern.PhotoUrl });
    }
}
