using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [Authorize(Roles = "Staff")]
public class BarangayOfficialController(IUnitOfWork unit, IMapper mapper) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BarangayOfficialDto>>> GetBarangayOfficials()
    {
        var officials = await unit.Repository<BarangayOfficial>().ListAllAsync();

        var sorted = officials.OrderBy(o => o.Rank).ToList();

        return Ok(mapper.Map<IReadOnlyList<BarangayOfficial>, IReadOnlyList<BarangayOfficialDto>>(sorted));
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<BarangayOfficialDto>>> GetActiveOfficials()
    {
        var spec = new ActiveBarangayOfficialsSpecifications();
        var officials = await unit.Repository<BarangayOfficial>().ListAsync(spec);

        return Ok(mapper.Map<IReadOnlyList<BarangayOfficial>, IReadOnlyList<BarangayOfficialDto>>(officials));
    }

    [HttpPost]
    public async Task<ActionResult<BarangayOfficialDto>> CreateOfficial(CreateOfficialDto dto)
    {
        var official = mapper.Map<BarangayOfficial>(dto);
        unit.Repository<BarangayOfficial>().Add(official);

        if (await unit.Complete()) return Ok(mapper.Map<BarangayOfficialDto>(official));

        return BadRequest("Failed to add barangay official");
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateOfficial(int id, UpdateOfficialDto dto)
    {
        var official = await unit.Repository<BarangayOfficial>().GetByIdAsync(id);
        if (official == null) return NotFound();

        mapper.Map(dto, official);
        unit.Repository<BarangayOfficial>().Update(official);

        if (await unit.Complete()) return NoContent();

        return BadRequest("Failed to update barangay official");
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<ActionResult> ToggleActive(int id)
    {
        var official = await unit.Repository<BarangayOfficial>().GetByIdAsync(id);
        if (official == null) return NotFound();

        official.IsActive = !official.IsActive;
        unit.Repository<BarangayOfficial>().Update(official);

        if (await unit.Complete()) return Ok(new { IsActive = official.IsActive });

        return BadRequest("Failed to toggle active status");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteOfficial(int id)
    {
        var official = await unit.Repository<BarangayOfficial>().GetByIdAsync(id);
        if (official == null) return NotFound();

        unit.Repository<BarangayOfficial>().Remove(official);
        
        if (await unit.Complete()) return NoContent();

        return BadRequest("Failed to delete barangay official");
    }

    [HttpPost("{id}/upload-image")]
    public async Task<ActionResult<BarangayOfficialDto>> UploadOfficialImage(int id, 
        IFormFile file, [FromQuery] string type = "photo")
    {
        var official = await unit.Repository<BarangayOfficial>().GetByIdAsync(id);
        if (official == null) return NotFound();

        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        var folderName = type == "signature" ? "signatures" : "officials";
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);

        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(directoryPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/images/{folderName}/{fileName}";

        if (type == "signature")
        {
            official.SignatureImage = relativePath;
        } else
        {
            official.OfficeImage = relativePath;
        }

        unit.Repository<BarangayOfficial>().Update(official);
        await unit.Complete();

        return Ok(mapper.Map<BarangayOfficialDto>(official));
    }
}
