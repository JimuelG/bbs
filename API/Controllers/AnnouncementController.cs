using API.DTOs;
using API.RequestHelpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class AnnouncementController(IUnitOfWork unit,
    IMapper mapper,
    ITtsService ttsService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AnnouncementDto>>> GetAnnouncements([FromQuery] AnnouncementSpecParams specParams)
    {
        var spec = new AnnouncementSpecification(specParams);
        var countSpec = new AnnouncementSpecification(specParams);

        var totalItems = await unit.Repository<Announcement>().CountAsync(countSpec);
        var announcements = await unit.Repository<Announcement>().ListAsync(spec);

        if (announcements == null || announcements.Count == 0)
        {
            return Ok(new List<Announcement>());
        }

        var data = mapper.Map<IReadOnlyList<Announcement>, IReadOnlyList<AnnouncementDto>>(announcements);

        return Ok(new Pagination<AnnouncementDto>(
            specParams.PageIndex,
            specParams.PageSize,
            totalItems,
            data
        ));
    }

    [HttpGet("latest")]
    public async Task<ActionResult<Announcement>> GetLatest()
    {
        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));

        var spec = new LatestAnnouncementSpecification(phTime);
        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec);
            
        if (announcement == null) return NoContent();

        announcement.IsPlayed = true;
        unit.Repository<Announcement>().Update(announcement);
        await unit.Complete();
        
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

    [HttpPost("trigger/{id}")]
    public async Task<IActionResult> TriggerManual(int id)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound();

        var spec = new AnnouncementManualTriggerSpecification();

        var pending = await unit.Repository<Announcement>().ListAsync(spec);

        foreach (var item in pending)
        {
            item.ManualTriggerActive = false;
        }

        announcement.ManualTriggerActive = true;
        announcement.IsPlayed = false;

        if (await unit.Complete())
        {
            return Ok(new { message = "Manual trigger activated for RPi"});
        }
        
        return BadRequest("Problem activating manual trigger");
        
    }

    [HttpGet("manual-trigger")]
    public async Task<ActionResult<Announcement>> GetManualTrigger()
    {
        var spec = new AnnouncementManualTriggerSpecification();

        var announcement = await unit.Repository<Announcement>().GetEntityWithSpec(spec);

        if (announcement == null) return NoContent();

        announcement.ManualTriggerActive = false;

        unit.Repository<Announcement>().Update(announcement);

        await unit.Complete();

        return Ok(announcement);
    }

    // [HttpGet("latest-emergency")]
    // public async Task<ActionResult> EmergencyTrigger()
    // {
    //     return Ok(new { message = "Emergency Triggered"});
    // }

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
        var phTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time"));
        var spec = new LatestAnnouncementSpecification(phTime);
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

    // [Authorize(Roles = "Staff")]
    [HttpPost]
    public async Task<ActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
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

    [HttpPost("preview")]
    public async Task<ActionResult> PreviewAnnouncement([FromBody] PreviewDto dto)
    {
        var audioUrl = await ttsService.GenerateSpeechAsync(dto.Message, dto.IsEmergency, dto.LanguageCode);

        return Ok(new { AudioUrl = audioUrl });
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAnnouncement(int id)
    {
        var announcement = await unit.Repository<Announcement>().GetByIdAsync(id);
        if (announcement == null) return NotFound();

        unit.Repository<Announcement>().Remove(announcement);
        await unit.Complete();

        return NoContent();
    }

    [HttpPost("heartbeat")]
    public async Task<IActionResult> Ping()
    {
        var statusList = await unit.Repository<RPiStatus>().ListAllAsync();
        var status = statusList.FirstOrDefault();

        if (status == null)
        {
            status = new RPiStatus { LastSeen = DateTime.UtcNow };
            unit.Repository<RPiStatus>().Add(status);
        }
        else
        {
            status.LastSeen = DateTime.UtcNow;
            unit.Repository<RPiStatus>().Update(status);
        }

        await unit.Complete();
        return Ok();
    }

    [HttpGet("rpi-status")]
    public async Task<ActionResult<object>> GetRPiStatus()
    {
        var statuses = await unit.Repository<RPiStatus>().ListAllAsync();
        var rpi = statuses.FirstOrDefault();

        if (rpi == null) return Ok(new { isOnline = false, lastSeen = (DateTime?)null });

        var diff = DateTime.UtcNow - rpi.LastSeen;
        var isOnline = Math.Abs(diff.TotalSeconds) < 90;

        var now = DateTime.UtcNow;
        var last = rpi.LastSeen;
        var secondsGap = (now - last).TotalSeconds;

        // Check your terminal/console to see these values
        Console.WriteLine($"Current UTC: {now}, RPi LastSeen: {last}, Gap: {secondsGap}");

        return Ok(new
        {
            isOnline = isOnline,
            lastSeen = rpi.LastSeen
        });
    }

}