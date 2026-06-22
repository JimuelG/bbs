using System.Formats.Asn1;
using System.Security.Claims;
using API.DTOs;
using API.RequestHelpers;
using AutoMapper;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class ConcernsController(
    IUnitOfWork unit, 
    IMapper mapper,
    IConcernPriorityService priorityService,
    UserManager<AppUser> userManager) 
    : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConcernDto>>> GetConcerns([FromQuery] ConcernParams specParams)
    {
        var spec = new ConcernSpecification(specParams);
        var countSpec = new ConcernCountSpecification(specParams);

        var totalItems = await unit.Repository<Concern>().CountAsync(countSpec);
        var concerns = await unit.Repository<Concern>().ListAsync(spec);

        var data = mapper.Map<IReadOnlyList<Concern>, IReadOnlyList<ConcernDto>>(concerns);

        return Ok(new Pagination<ConcernDto>(
            specParams.PageIndex,
            specParams.PageSize,
            totalItems,
            data
        ));
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
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return Unauthorized("User email claim not found.");
        
        var user = await userManager.Users
            .Include(x => x.Resident)
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
            return Unauthorized("User not found");

        if (user.Resident == null)
            return BadRequest("No resident profile is linked to this account.");

        var concern = mapper.Map<Concern>(dto);

        concern.ResidentId = user.Resident.Id;
        concern.DateReported = DateTime.UtcNow;
        concern.Status = ConcernStatus.Pending;

        concern.Priority = priorityService.DeterminePriority(
            dto.Type,
            dto.Title,
            dto.Description
        );

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

        if (concern.Status == ConcernStatus.Resolved || concern.Status == ConcernStatus.Dismissed)
        {
            return BadRequest(new { message = "This concern is already closed and can no longer be updated." });
        }

        if (Enum.TryParse<ConcernStatus>(dto.Status, out var parsedStatus))
        {
            concern.Status = parsedStatus;
    
            if (parsedStatus == ConcernStatus.Resolved || parsedStatus == ConcernStatus.Dismissed)
            {
                concern.DateResolved = DateTime.UtcNow;
            }
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
    public async Task<ActionResult> UploadPhoto(int id, [FromForm] IFormFile file)
    {
        var concern = await unit.Repository<Concern>().GetByIdAsync(id);
        if (concern == null) return NotFound("Concern not found");

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Invalid file type. Only JPG and PNG are allowed." });

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "images", "concerns");

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
