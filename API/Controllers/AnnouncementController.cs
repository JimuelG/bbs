using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class AnnouncementController(IUnitOfWork unit,
    IMapper mapper,
    ITtsService ttsService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AnnouncementDto>>> GetAnnouncements()
    {
        var announcement = await unit.Repository<Announcement>().ListAllAsync();

        if (announcement == null) return NoContent();

        return Ok( mapper.Map<IReadOnlyList<AnnouncementDto>>(announcement));
    }

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

    [Authorize(Roles = "Staff")]
    [HttpPost]
    public async Task<ActionResult> CreateAnnouncement(CreateAnnouncementDto dto)
    {
        var audioUrl = await ttsService.GenerateSpeechAsync(dto.Message, dto.IsEmergency, dto.LanguageCode);

        var announcement = mapper.Map<Announcement>(dto);
        announcement.AudioUrl = audioUrl;

        unit.Repository<Announcement>().Add(announcement);
        await unit.Complete();

        return CreatedAtAction(nameof(GetAnnounceById), 
            new { id = announcement.Id}, 
            mapper.Map<AnnouncementDto>(announcement));
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