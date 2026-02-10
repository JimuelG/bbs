using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class AnnouncementController(IUnitOfWork unit, UserManager<AppUser> userManager, IMapper mapper) : BaseApiController
{

    [HttpGet("latest")]
    public async Task<ActionResult<Announcement>> GetLatest()
    {
        var spec = new LatestAnnouncementSpecification();
        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec);
            
        if (announcement == null) return NoContent();

        var dto = new AnnouncementResponseDto
        {
            Id = announcement.Id,
            Message = announcement.Message,
            AudioUrl = announcement.AudioUrl,
            IsEmergency = announcement.IsEmergency,
            ScheduledAt = announcement.ScheduledAt
        };

        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementDto>> GetAnnounceById(int id)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);

        if (announcement == null) return NotFound("No announcement found");

        var mapped = mapper.Map<AnnouncementDto>(announcement);

        return Ok(mapped);
    }

    [HttpGet("display")]
    public async Task<ActionResult<DisplayAnnouncementDto>> GetDisplay()
    {
        var spec = new LatestAnnouncementSpecification();
        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec); 
        if (announcement == null) return NoContent();

        var dto = new DisplayAnnouncementDto
        {
            Title = announcement.Title,
            Message = announcement.Message,
            IsEmergency = announcement.IsEmergency

        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAnnouncement(CreateAnnouncementDto dto)
    {
        var announcement = new Announcement
        {
            Title = dto.Title,
            Message = dto.Message,
            ScheduledAt = dto.ScheduledAt,
            ExpireAt = dto.ExpireAt,
            IsEmergency = dto.IsEmergency
        };

        unit.Repository<Announcement>().Add(announcement);
        await unit.Complete();

        var mapped = mapper.Map<AnnouncementResponseDto>(announcement);

        return Ok(mapped);
    }

    [HttpPut("{id}/played")]
    public async Task<ActionResult> MarkAsPlayed(int id)
    {

        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound();

        announcement.IsPlayed = true;
        unit.Repository<Announcement>().Update(announcement);
        await unit.Complete();

        return NoContent();
    }

}